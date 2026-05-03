using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.Domain.Rules;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.Exceptions;
using Fixi.Core.Mappings;
using Fixi.Core.ServicesContracts;
using Fixi.Core.ServicesContracts.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace Fixi.Core.Services
{
    public class TicketService : ITicketService
    {
        IUnitOfWork _unitOfWork;
        IIdentityService _identityService;
        IBackgroundJobService _backgroundJobService;
        public TicketService(IUnitOfWork unitOfWork, IIdentityService identityService, IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _backgroundJobService = backgroundJobService;
        }


        public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticketDTO,string UserID)
        {
            SLASetting? slaSetting = await _unitOfWork.SLASetting.GetByPriorityAsync(ticketDTO.Priority);
            if (slaSetting == null)
            {
                throw new ValidationException("SLA settings not found for the specified priority.");
            }

            Ticket ticket = ticketDTO.ToEntity();
            ticket.Status = TicketStatus.Open;
            ticket.ReportedById = UserID;
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.LastModifiedDate = DateTime.UtcNow;
            ticket.SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResolutionTimeMinutes);
            ticket.SLAResponseDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResponseTimeMinutes);
            ticket.LastModifiedById = UserID;

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
            var emails = await _unitOfWork.Ticket.GetTicketUsersEmailsAsync(ticket.Id);

            if (!string.IsNullOrEmpty(emails.ManagerEmail))
            {
                _backgroundJobService.SendEmail(emails.ManagerEmail, "New Ticket Created", $"A new ticket has been created with ID {ticket.Id}. Please check the system for details.");
            }

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

        public async Task UpdateTicketAsync(Ticket updateTicket, UserClaims claims)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(updateTicket.Id);

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
                TicketId = updateTicket.Id,
                ChangedById = claims.UserId,
                ChangeType = "Updated Ticket",
                ChangedDate = DateTime.UtcNow,
            });
            await _unitOfWork.Ticket.UpdateAsync(updateTicket);
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

        public async Task UpdateTicketPriority(int ticketId, int newPriority, string userID )
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
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

        public async Task UpdateTicketStatus(int ticketId, TicketUpdateStatusDTO statusDTO, UserClaims claims)
        {
            TicketDTO? ticket =  await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            RoleEnum Role = (RoleEnum)Enum.Parse(typeof(RoleEnum), claims.Role);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if(!TicketStatusRules.TransitionRules.Any(x => x.From == ticket.status && x.Role == Role && x.To == (TicketStatus)statusDTO.NewStatus))
            {
                throw new BusinessRuleViolationException($"User with role {claims.Role} is not allowed to change status from {ticket.status} to {(TicketStatus)statusDTO.NewStatus}.");
            }

            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = claims.UserId,
                ChangeType = "Updated Status",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.status.ToString(),
                NewValue = ((TicketStatus)statusDTO.NewStatus).ToString(),
                ChangeReason = statusDTO.Comment

            });
            await _unitOfWork.Ticket.UpdateStatus(ticketId, (TicketStatus)statusDTO.NewStatus);
            await _unitOfWork.CommitAsync();
            await SendEmail((TicketStatus)statusDTO.NewStatus, ticketId);
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
            if(NewAssigned.Id != calims.UserId && !string.IsNullOrEmpty(NewAssigned.Email))
            {
               _backgroundJobService.SendEmail(NewAssigned.Email, "New Ticket Assignment", $"You have been assigned to ticket, With priority: {ticket.priority}. Please check the system for details.");
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

        public async Task UpdateSLAStatusesAsync()
        {
            var responseBreachedTicketIds = await _unitOfWork.Ticket.GetTicketIdsResponseDeadlineBreachedAsync();
            foreach (var ticketId in responseBreachedTicketIds)
            {
                var emails = await _unitOfWork.Ticket.GetTicketUsersEmailsAsync(ticketId);
                await _unitOfWork.Ticket.UpdateSLAResponseBreachedStatus(ticketId);
                _backgroundJobService.SendEmail(emails.TechnicianEmail, "SLA Response Breached", "The SLA response time for your ticket has been breached.");
            }
            var resolutionBreachedTicketIds = await _unitOfWork.Ticket.GetTicketIdsResolutionDeadlineBreachedAsync();
            foreach (var ticketId in resolutionBreachedTicketIds)
            {
                var emails = await _unitOfWork.Ticket.GetTicketUsersEmailsAsync(ticketId);
                await _unitOfWork.Ticket.UpdateSLAResolutionStatus(ticketId);
                _backgroundJobService.SendEmail(emails.TechnicianEmail, "SLA Resolution Breached", "The SLA resolution time for your ticket has been breached.");
            }
        }

        private async Task SendEmail(TicketStatus status,int ticketId)
        {
            TicketUsersEmails? usersEmails = await _unitOfWork.Ticket.GetTicketUsersEmailsAsync(ticketId);
            if (usersEmails == null)
            {
                return;
            }
            switch (status)
            {
                case TicketStatus.Resolved:
                    if (string.IsNullOrEmpty(usersEmails.ReporterEmail))
                    {
                        _backgroundJobService.SendEmail(usersEmails.ReporterEmail, "Ticket Resolved", $"Your ticket with ID {ticketId} has been resolved. Please check the system for details.");
                        return;
                    }
                    break;
                case TicketStatus.Canceled:
                    if (string.IsNullOrEmpty(usersEmails.TechnicianEmail))
                    {
                        _backgroundJobService.SendEmail(usersEmails.TechnicianEmail, "Ticket Canceled", $"ticket with ID {ticketId} has been canceled. Please check the system for details.");
                        return;
                    }
                    break;
                default:
                    return;
            }
        }


    }
}
