using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.DTOs.TicketDTOs;
using DeskFkow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DeskFkow.Core.ServicesContracts
{
    public interface ITicketService
    {
        public Task<Ticket> CreateTicketAsync(CreateTicketDTO createTicketDTO);
        public Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams);
        public Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId);

        public Task UpdateTicketAsync(Ticket updateTicket);
        public Task UpdateTicketStatus(int ticketId, TicketUpdateStatusDTO statusDTO);
        public Task UpdateTicketPriority(int ticketId, int newPriority);
        public Task AssignTechnician(int ticketId, string newtechnicianId);
        public Task DeleteTicket(int ticketId);

        public Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHistoryAsync(int ticketId);

        public Task UpdateSLAStatusesAsync();

    }
}
