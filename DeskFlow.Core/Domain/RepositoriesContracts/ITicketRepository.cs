using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.TicketDTOs;
using DeskFkow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface ITicketRepository
    {
        Task<TicketFullResponseDTO?> GetFullTicketAsync(int ticketId);
        Task<IEnumerable<TicketResponseDTO>> GetAllAsync(TicketQueryParams queryParams);
        Task<Ticket> CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int ticketId);
        Task UpdateStatus(int ticketId, TicketStatus newStatus, string userID);
        Task UpdatePriority(int ticketId, int newPriority, string userID);
        Task AssignTechnician(int ticketId, string technicianId, string userID);
        Task<TicketDTO?> GetTicketAsync(int ticketId);
        Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHisoryAsync(int ticketId);
        Task<TicketUsersEmails> GetTicketUsersEmailsAsync(int ticketId);
        Task UpdateSLAResponseBreachedStatus(int ticketId);
        Task UpdateSLAResolutionStatus(int ticketId);
        Task <IEnumerable<int>> GetTicketIdsResponseDeadlineBreachedAsync();
        Task <IEnumerable<int>> GetTicketIdsResolutionDeadlineBreachedAsync();
    }
}
