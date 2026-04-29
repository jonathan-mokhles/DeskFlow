using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using Hangfire;
using System.ComponentModel.DataAnnotations;

namespace Fixi.Core.Services
{
    public class TicketService : ITicketService
    {
        IUnitOfWork _unitOfWork;
        IIdentityService _identityService;
        IMailService _mailService;
        public TicketService(IUnitOfWork unitOfWork, IIdentityService identityService, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mailService = mailService;
        }


        public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticketDTO,string UserID)
        {
            SLASetting? slaSetting = await _unitOfWork.SLASetting.GetByPriorityAsync(ticketDTO.Priority);
            if (slaSetting == null)
            {
                throw new ValidationException("SLA settings not found for the specified priority.");
            }

            Ticket ticket = new Ticket
            {
                Title = ticketDTO.Title,
                Description = ticketDTO.Description,
                Priority = (TicketPriority)ticketDTO.Priority,
                Status = TicketStatus.Open,
                CategoryId = ticketDTO.CategoryId,
                ReportedById = UserID,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResolutionTimeMinutes),
                SLAResponseDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResponseTimeMinutes),
                LastModifiedById = UserID

            };

            await _unitOfWork.Ticket.CreateAsync(ticket);

            TicketAuditLog auditLog = new TicketAuditLog
            {
                Ticket = ticket,
                ChangedById = UserID,
                ChangeType = "Created",
                ChangedDate = ticket.CreatedDate,
                OldValue = null,
                NewValue = $"Title: {ticket.Title}, Description: {ticket.Description}, Priority: {ticket.Priority}, Status: {ticket.Status}, CategoryId: {ticket.CategoryId}"
            };
            await _unitOfWork.TicketAuditLog.CreateAsync(auditLog);
            await _unitOfWork.CommitAsync();

            return ticket;

        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams, UserClaims claims)
        {
            if(claims.Role == nameof(RoleEnum.Manager) || claims.Role == nameof(RoleEnum.Technician))
            {
                queryParams.DepartmentId = claims.DeptId;
            }
            else if(claims.Role == nameof(RoleEnum.User))
            {
                queryParams.ReporterId = claims.UserId;
            }
            return await _unitOfWork.Ticket.GetAllAsync(queryParams);
        }

        public async Task UpdateTicketAsync(UpdateTicketDTO updateTicketDTO, UserClaims claims)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(updateTicketDTO.Id);

            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if ( ticket.ReportedById != claims.UserId )
            {
                throw new UnauthorizedTicketAccessException();
            }

            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = updateTicketDTO.Id,
                ChangedById = claims.UserId,
                ChangeType = "Updated Ticket",
                ChangedDate = DateTime.UtcNow,
            });
            await _unitOfWork.Ticket.UpdateAsync(updateTicketDTO);
            await _unitOfWork.CommitAsync();
        }

        public async Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId)
        {
            var ticket =  await _unitOfWork.Ticket.GetFullTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            return ticket;
        }

        public async Task UpdateTicketPriority(TicketDTO ticket, int newPriority, string userID )
        {

            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = userID,
                ChangeType = "Updated Priority",
                ChangedDate = DateTime.UtcNow,
                OldValue = ((TicketPriority)ticket.priority).ToString(),
                NewValue = ((TicketPriority)newPriority).ToString()
            });
            await _unitOfWork.Ticket.UpdatePriority(ticket.Id, newPriority);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateTicketStatus(int ticketId, TicketStatus newStatus, UserClaims claims)
        {
            TicketDTO? ticket =  await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            string Role = claims.Role;
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if (ticket.DepartmentId != claims.DeptId || (claims.UserId != ticket.AssignedToId && ticket.ReportedById != claims.UserId && Role != nameof(RoleEnum.Manager)))
            {
                throw new UnauthorizedTicketAccessException();
            }
            if (!allowedTransitions[ticket.status].Contains(newStatus))
            {
                throw new BusinessRuleViolationException($"Invalid status transition");
            }
            if((ticket.status == TicketStatus.InProgress || newStatus == TicketStatus.InProgress) && Role != nameof(RoleEnum.Technician))
            {
                throw new BusinessRuleViolationException("Only technicians can move tickets to or from In Progress status.");
            }  
            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = claims.UserId,
                ChangeType = "Updated Status",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.status.ToString(),
                NewValue = newStatus.ToString()
            });
            await _unitOfWork.Ticket.UpdateStatus(ticketId, newStatus);
            await _unitOfWork.CommitAsync();
        }

        public async Task AssignTechnician(int ticketId, string newtechnicianId, UserClaims calims)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if (ticket.DepartmentId != calims.DeptId)
            {
                throw new UnauthorizedTicketAccessException();
            }
            if (calims.Role == nameof(RoleEnum.Technician) && newtechnicianId != calims.UserId)
            {
                throw new BusinessRuleViolationException("Technician can only assign themselves to a ticket.");
            }
            ApplicationUser? NewAssigned = await _identityService.FindByIdAsync(newtechnicianId);
            if (NewAssigned == null)
            {
                throw new NotFoundException("Technician not found.");
            }
            if(NewAssigned.DepartmentId != calims.DeptId)
            {
                throw new BusinessRuleViolationException("Cannot assign technician from a different department.");
            }
            
            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = calims.UserId,
                ChangeType = "Assigned Technician",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.AssignedToFullname != null ? ticket.AssignedToFullname : "Unassigned",
                NewValue = NewAssigned.FullName

            });
            await _unitOfWork.Ticket.AssignTechnician(ticketId, newtechnicianId);
            await _unitOfWork.CommitAsync();
            if(NewAssigned.Id != calims.UserId)
            {
                BackgroundJob.Enqueue(() => _mailService.SendEmailAsync(NewAssigned.Email, "New Ticket Assignment", $"You have been assigned to ticket, With priority: {ticket.priority}. Please check the system for details."));
            }
        }

        public async Task DeleteTicket(int ticketId, string userId)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            TicketAuditLog auditLog = new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = userId,
                ChangeType = "Deleted",
                ChangedDate = DateTime.UtcNow,
                OldValue = $"Priority: {ticket.priority}, Status: {ticket.status}",
                NewValue = null
            };
            await _unitOfWork.TicketAuditLog.CreateAsync(auditLog);
            await _unitOfWork.Ticket.DeleteAsync(ticketId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHistoryAsync(int ticketId)
        {
            return await _unitOfWork.Ticket.GetTicketHisoryAsync(ticketId);
        }



        private static Dictionary<TicketStatus, List<TicketStatus>> allowedTransitions = new Dictionary<TicketStatus, List<TicketStatus>>
        {
            { TicketStatus.Open, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Canceled } },
            { TicketStatus.InProgress, new List<TicketStatus> { TicketStatus.Resolved, TicketStatus.OnHold } },
            {TicketStatus.OnHold, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Canceled }  },
            { TicketStatus.Resolved, new List<TicketStatus> { TicketStatus.Closed, TicketStatus.InProgress } },
            { TicketStatus.Closed, new List<TicketStatus>() },
            { TicketStatus.Canceled, new List<TicketStatus>() },
        };

    }
}
