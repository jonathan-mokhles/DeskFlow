using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface ITicketService
    {
        public Task<Ticket> CreateTicketAsync(CreateTicketDTO createTicketDTO,string UserID);
        public Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams, UserClaims claims);
        public Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId);

        public Task UpdateTicketAsync(UpdateTicketDTO updateTicketDTO, UserClaims claims);
        public Task UpdateTicketStatus(int ticketId, TicketUpdateStatusDTO statusDTO, UserClaims calims);
        public Task UpdateTicketPriority(TicketDTO ticket, int newPriority, string userID);
        public Task AssignTechnician(int ticketId, string newtechnicianId, UserClaims calims);
        public Task DeleteTicket(int ticketId, string userId);

        public Task<IEnumerable<TicketAuditHistoryDTO>> GetTicketHistoryAsync(int ticketId);

        public Task UpdateSLAStatusesAsync();

    }
}
