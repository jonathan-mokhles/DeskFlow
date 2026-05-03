using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.Domain.Rules;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.DTOs.TicketDTOs;
using DeskFkow.Core.Enums;
using DeskFkow.Core.Exceptions;
using DeskFkow.Core.Mappings;
using DeskFkow.Core.ServicesContracts;
using DeskFkow.Core.ServicesContracts.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace DeskFkow.Core.Services
{
    public class TicketService : ITicketService
    {
        IUnitOfWork _unitOfWork;
        IIdentityService _identityService;
        IBackgroundJobService _backgroundJobService;
        ICurrentUserService _currentUser;
        public TicketService(IUnitOfWork unitOfWork, IIdentityService identityService, IBackgroundJobService backgroundJobService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _backgroundJobService = backgroundJobService;
            _currentUser = currentUserService;
        }


        public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticketDTO)
        {
            SLASetting? slaSetting = await _unitOfWork.SLASetting.GetByPriorityAsync(ticketDTO.Priority);
            if (slaSetting == null)
            {
                throw new ValidationException("SLA settings not found for the specified priority.");
            }

            Ticket ticket = ticketDTO.ToEntity();
            ticket.Status = TicketStatus.Open;
            ticket.ReportedById = _currentUser.UserId;
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.LastModifiedDate = DateTime.UtcNow;
            ticket.SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResolutionTimeMinutes);
            ticket.SLAResponseDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResponseTimeMinutes);
            ticket.LastModifiedById = _currentUser.UserId;

            await _unitOfWork.Ticket.CreateAsync(ticket);

            TicketAuditLog auditLog = new TicketAuditLog
            {
                Ticket = ticket,
                ChangedById = _currentUser.UserId,
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

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams)
        {
            if(_currentUser.Role == nameof(RoleEnum.Manager) || _currentUser.Role == nameof(RoleEnum.Technician))
            {
                queryParams.DepartmentId = _currentUser.DeptId;
            }
            else if(_currentUser.Role == nameof(RoleEnum.User))
            {
                queryParams.ReporterId = _currentUser.UserId;
            }
            return await _unitOfWork.Ticket.GetAllAsync(queryParams);
        }

        public async Task UpdateTicketAsync(Ticket updateTicket)
        {
            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = updateTicket.Id,
                ChangedById = updateTicket.LastModifiedById,
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

        public async Task UpdateTicketPriority(int ticketId, int newPriority)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = _currentUser.UserId,
                ChangeType = "Updated Priority",
                ChangedDate = DateTime.UtcNow,
                OldValue = ((TicketPriority)ticket.priority).ToString(),
                NewValue = ((TicketPriority)newPriority).ToString()
            });
            await _unitOfWork.Ticket.UpdatePriority(ticket.Id, newPriority, _currentUser.UserId);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateTicketStatus(int ticketId, TicketUpdateStatusDTO statusDTO)
        {
            TicketDTO? ticket =  await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            RoleEnum Role = (RoleEnum)Enum.Parse(typeof(RoleEnum), _currentUser .Role);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }

            if(!TicketStatusRules.TransitionRules.Any(x => x.From == ticket.status && x.Role == Role && x.To == (TicketStatus)statusDTO.NewStatus))
            {
                throw new BusinessRuleViolationException($"User with role {_currentUser.Role} is not allowed to change status from {ticket.status} to {(TicketStatus)statusDTO.NewStatus}.");
            }

            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = _currentUser.UserId,
                ChangeType = "Updated Status",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.status.ToString(),
                NewValue = ((TicketStatus)statusDTO.NewStatus).ToString(),
                ChangeReason = statusDTO.Comment

            });
            await _unitOfWork.Ticket.UpdateStatus(ticketId, (TicketStatus)statusDTO.NewStatus, _currentUser.UserId);
            await _unitOfWork.CommitAsync();
            await SendEmail((TicketStatus)statusDTO.NewStatus, ticketId);
        }

        public async Task AssignTechnician(int ticketId, string newtechnicianId)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            if (_currentUser.Role == nameof(RoleEnum.Technician) && newtechnicianId != _currentUser.UserId)
            {
                throw new BusinessRuleViolationException("Technician can only assign themselves to a ticket.");
            }

            ApplicationUser? NewAssigned = await _identityService.FindByIdAsync(newtechnicianId);
            if (NewAssigned == null)
            {
                throw new NotFoundException("Technician not found.");
            }
            if(NewAssigned.DepartmentId != _currentUser.DeptId)
            {
                throw new BusinessRuleViolationException("Cannot assign technician from a different department.");
            }
            
            await _unitOfWork.TicketAuditLog.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = _currentUser.UserId,
                ChangeType = "Assigned Technician",
                ChangedDate = DateTime.UtcNow,
                OldValue = ticket.AssignedToFullname != null ? ticket.AssignedToFullname : "Unassigned",
                NewValue = NewAssigned.FullName

            });
            await _unitOfWork.Ticket.AssignTechnician(ticketId, newtechnicianId, _currentUser.UserId);
            await _unitOfWork.CommitAsync();
            if(NewAssigned.Id != _currentUser.UserId && !string.IsNullOrEmpty(NewAssigned.Email))
            {
               _backgroundJobService.SendEmail(NewAssigned.Email, "New Ticket Assignment", $"You have been assigned to ticket, With priority: {ticket.priority}. Please check the system for details.");
            }
        }

        public async Task DeleteTicket(int ticketId)
        {
            TicketDTO? ticket = await _unitOfWork.Ticket.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new TicketNotFoundException();
            }
            TicketAuditLog auditLog = new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = _currentUser.UserId,
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
