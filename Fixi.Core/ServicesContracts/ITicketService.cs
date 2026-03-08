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
        public Task<Ticket> CreateTicketAsync(CreateTicketDTO createTicketDTO);
        public Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync(TicketQueryParams queryParams, UserClaims claims);

        public Task<TicketFullResponseDTO> GetTicketByIdAsync(int ticketId, UserClaims calims);

        public Task UpdateTicketStatus(int ticketId, TicketStatus newStatus, UserClaims calims);
        public Task UpdateTicketPriority(int ticketId, int newPriority, UserClaims calims);
        public Task AssignTechnician(int ticketId, string newtechnicianId, UserClaims calims);

        public Task DeleteTicket(int ticketId, string userId);

    }
}
