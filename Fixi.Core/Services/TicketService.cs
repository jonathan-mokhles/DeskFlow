using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using System.ComponentModel.DataAnnotations;

namespace Fixi.Core.Services
{
    public class TicketService : ITicketService
    {
        ITicketRepository _ticketRepository;
        ISLASettingRepository _slaRepo;
        ITicketAuditLogRepository _ticketAuditLogRepo;
        IIdentityService _identityService;
        public TicketService(ITicketRepository ticketRepository, ISLASettingRepository sLASetting, ITicketAuditLogRepository ticketAudit, IIdentityService identityService)
        {
            _ticketRepository = ticketRepository;
            _slaRepo = sLASetting;
            _ticketAuditLogRepo = ticketAudit;
            _identityService = identityService;
        }


        public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticketDTO)
        {
            SLASetting? slaSetting = await _slaRepo.GetByPriorityAsync(ticketDTO.Priority);
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
                ReportedById = ticketDTO.ReportedById,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResolutionTimeMinutes),
                SLAResponseDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResponseTimeMinutes),
                LastModifiedById = ticketDTO.ReportedById

            };

            await _ticketRepository.CreateAsync(ticket);

            TicketAuditLog auditLog = new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = ticketDTO.ReportedById,
                ChangeType = "Created",
                ChangedDate = ticket.CreatedDate,
                OldValue = null,
                NewValue = $"Title: {ticket.Title}, Description: {ticket.Description}, Priority: {ticket.Priority}, Status: {ticket.Status}, CategoryId: {ticket.CategoryId}"
            };
            await _ticketAuditLogRepo.CreateAsync(auditLog);

            return ticket;

        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams, UserClaims claims)
        {
            if(claims.Role == "Manager" || claims.Role == "Technician")
            {
                queryParams.DepartmentId = claims.DeptId;
            }
            else if(claims.Role == "User")
            {
                queryParams.ReporterId = claims.UserId;
            }
            return await _ticketRepository.GetAllAsync(queryParams);
        }

        public async Task UpdateTicketAsync(UpdateTicketDTO updateTicketDTO, UserClaims claims)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(updateTicketDTO.Id);

            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if ( ticket.ReportedById != claims.UserId )
            {
                throw new UnauthorizedTicketAccessException();
            }

            await _ticketAuditLogRepo.CreateAsync(new TicketAuditLog
            {
                TicketId = updateTicketDTO.Id,
                ChangedById = claims.UserId,
                ChangeType = "Updated Ticket",
                ChangedDate = DateTime.UtcNow,
            });
            await _ticketRepository.UpdateAsync(updateTicketDTO);
        }

        public async Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId)
        {
            var ticket =  await _ticketRepository.GetFullTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            return ticket;
        }

        public async Task UpdateTicketPriority(TicketDTO ticket, int newPriority, string userID )
        {

            await _ticketAuditLogRepo.CreateAsync(new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = userID,
                ChangeType = "Updated Priority",
                ChangedDate = DateTime.UtcNow,
                OldValue = ((TicketPriority)ticket.priority).ToString(),
                NewValue = ((TicketPriority)newPriority).ToString()
            });
            await _ticketRepository.UpdatePriority(ticket.Id, newPriority);
        }

        public async Task UpdateTicketStatus(int ticketId, TicketStatus newStatus, UserClaims claims)
        {
            TicketDTO? ticket =  await _ticketRepository.GetTicketAsync(ticketId);
            string Role = claims.Role;
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if (ticket.DepartmentId != claims.DeptId || (claims.UserId != ticket.AssignedToId && ticket.ReportedById != claims.UserId && Role != "Manager"))
            {
                throw new UnauthorizedTicketAccessException();
            }
            if (!allowedTransitions[ticket.status].Contains(newStatus))
            {
                throw new BusinessRuleViolationException($"Invalid status transition");
            }
            if((ticket.status == TicketStatus.InProgress || newStatus == TicketStatus.InProgress) && Role != "Technician")
            {
                throw new BusinessRuleViolationException("Only technicians can move tickets to or from In Progress status.");
            }  
            await _ticketAuditLogRepo .CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = claims.UserId,
                ChangeType = "Updated Status",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.status.ToString(),
                NewValue = newStatus.ToString()
            });
            await _ticketRepository.UpdateStatus(ticketId, newStatus);
        }

        public async Task AssignTechnician(int ticketId, string newtechnicianId, UserClaims calims)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if (ticket.DepartmentId != calims.DeptId)
            {
                throw new UnauthorizedTicketAccessException();
            }
            if (calims.Role == "Technician" && newtechnicianId != calims.UserId)
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
            
            await _ticketAuditLogRepo.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = calims.UserId,
                ChangeType = "Assigned Technician",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.AssignedToFullname != null ? ticket.AssignedToFullname : "Unassigned",
                NewValue = NewAssigned.FullName

            });
            await _ticketRepository.AssignTechnician(ticketId, newtechnicianId);
        }

        public async Task DeleteTicket(int ticketId, string userId)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
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
            await _ticketAuditLogRepo.CreateAsync(auditLog);
            await _ticketRepository.DeleteAsync(ticketId);
        }

        public async Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHistoryAsync(int ticketId)
        {
            return await _ticketRepository.GetTicketHisoryAsync(ticketId);
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
