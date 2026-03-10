using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Fixi.Core.Services
{
    public class TicketService : ITicketService
    {
        ITicketRepository _ticketRepository;
        ISLASettingRepository _slaRepo;
        ITicketAuditLogRepository _ticketAuditLogRepo;
        UserManager<ApplicationUser> _userManager;
        public TicketService(ITicketRepository ticketRepository, ISLASettingRepository sLASetting, ITicketAuditLogRepository ticketAudit, UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = ticketRepository;
            _slaRepo = sLASetting;
            _ticketAuditLogRepo = ticketAudit;
            _userManager = userManager;
        }


        public async Task<Ticket> CreateTicketAsync(CreateTicketDTO TicketDTO)
        {
            SLASetting slaSetting = await _slaRepo.GetByPriorityAsync(TicketDTO.Priority);
            if (slaSetting == null)
            {
                throw new Exception("SLA settings not found for the specified priority.");
            }

            Ticket ticket = new Ticket
            {
                Title = TicketDTO.Title,
                Description = TicketDTO.Description,
                Priority = (TicketPriority)TicketDTO.Priority,
                Status = TicketStatus.New,
                CategoryId = TicketDTO.CategoryId,
                ReportedById = TicketDTO.ReportedById,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                SLAResolutionDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResolutionTimeMinutes),
                SLAResponseDeadline = DateTime.UtcNow.AddMinutes(slaSetting.ResponseTimeMinutes),
                LastModifiedById = TicketDTO.ReportedById

            };
            await _ticketRepository.CreateAsync(ticket);

            TicketAuditLog auditLog = new TicketAuditLog
            {
                TicketId = ticket.Id,
                ChangedById = TicketDTO.ReportedById,
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
                throw new Exception("Ticket not found.");
            }
            if ( ticket.ReportedById != claims.UserId )
            {
                throw new Exception("Unauthorized to update this ticket.");
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

        public async Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId, UserClaims claims)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            string Role = claims.Role;
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }
            if (ticket.DepartmentId != claims.DeptId || (claims.UserId != ticket.AssignedToId && ticket.ReportedById != claims.UserId && Role != "Manager"))
            {
                throw new Exception("Unauthorized to update this ticket.");
            }
            return await _ticketRepository.GetFullTicketAsync(ticketId);

        }

        public async Task UpdateTicketPriority(int ticketId, int newPriority, UserClaims calims)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }
            if (ticket.DepartmentId != calims.DeptId)
            {
                throw new Exception("Unauthorized to update this ticket.");
            }
            await _ticketAuditLogRepo.CreateAsync(new TicketAuditLog
            {
                TicketId = ticketId,
                ChangedById = calims.UserId,
                ChangeType = "Updated Priority",
                ChangedDate = DateTime.UtcNow,
                OldValue = ((TicketPriority)ticket.priority).ToString(),
                NewValue = ((TicketPriority)newPriority).ToString()
            });
            await _ticketRepository.UpdatePriority(ticketId, newPriority);
        }

        public async Task UpdateTicketStatus(int ticketId, TicketStatus newStatus, UserClaims claims)
        {
            TicketDTO? ticket =  await _ticketRepository.GetTicketAsync(ticketId);
            string Role = claims.Role;
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }
            if (ticket.DepartmentId != claims.DeptId || (claims.UserId != ticket.AssignedToId && ticket.ReportedById != claims.UserId && Role != "Manager"))
            {
                throw new Exception("Unauthorized to update this ticket.");
            }
            if (!allowedTransitions[ticket.status].Contains(newStatus))
            {
                throw new Exception($"Invalid status transition");
            }
            if((ticket.status == TicketStatus.InProgress || newStatus == TicketStatus.InProgress) && Role != "Technician")
            {
                throw new Exception("Only technicians can move tickets to or from In Progress status.");
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
                throw new Exception("Ticket not found.");
            }
            if (ticket.DepartmentId != calims.DeptId)
            {
                throw new Exception("Unauthorized to update this ticket.");
            }
            if (calims.Role == "Technician" && newtechnicianId != calims.UserId)
            {
                throw new Exception("Technician can only assign themselves to a ticket.");
            }
            ApplicationUser? NewAssigned = await _userManager.FindByIdAsync(newtechnicianId);
            if (NewAssigned == null)
            {
                throw new Exception("Technician not found.");
            }
            if(NewAssigned.DepartmentId != calims.DeptId)
            {
                throw new Exception("Cannot assign technician from a different department.");
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
                throw new Exception("Ticket not found.");
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

        public async Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHistoryAsync(int ticketId, UserClaims claims)
        {
            TicketDTO? ticket = await _ticketRepository.GetTicketAsync(ticketId);
            string Role = claims.Role;
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }
            if (ticket.DepartmentId != claims.DeptId || (claims.UserId != ticket.AssignedToId && ticket.ReportedById != claims.UserId && Role != "Manager"))
            {
                throw new Exception("Unauthorized to view this ticket history.");
            }
            return await _ticketRepository.GetTicketHisoryAsync(ticketId);
        }


        private static Dictionary<TicketStatus, List<TicketStatus>> allowedTransitions = new Dictionary<TicketStatus, List<TicketStatus>>
        {
            { TicketStatus.New, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Canceled } },
            { TicketStatus.InProgress, new List<TicketStatus> { TicketStatus.Resolved, TicketStatus.OnHold } },
            {TicketStatus.OnHold, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Canceled }  },
            { TicketStatus.Resolved, new List<TicketStatus> { TicketStatus.Closed, TicketStatus.InProgress } },
            { TicketStatus.Closed, new List<TicketStatus>() },
            { TicketStatus.Canceled, new List<TicketStatus>() },
        };

    }
}
