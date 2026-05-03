using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketRepository
    {
        Task<TicketFullResponseDTO?> GetFullTicketAsync(int ticketId);
        Task<IEnumerable<TicketResponseDTO>> GetAllAsync(TicketQueryParams queryParams);
        Task<Ticket> CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int ticketId);
        Task UpdateStatus(int ticketId, TicketStatus newStatus);
        Task UpdatePriority(int ticketId, int newPriority);
        Task AssignTechnician(int ticketId, string technicianId);
        Task<TicketDTO?> GetTicketAsync(int ticketId);
        Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHisoryAsync(int ticketId);
        Task<TicketUsersEmails> GetTicketUsersEmailsAsync(int ticketId);
        Task UpdateSLAResponseBreachedStatus(int ticketId);
        Task UpdateSLAResolutionStatus(int ticketId);
        Task <IEnumerable<int>> GetTicketIdsResponseDeadlineBreachedAsync();
        Task <IEnumerable<int>> GetTicketIdsResolutionDeadlineBreachedAsync();
    }
}
