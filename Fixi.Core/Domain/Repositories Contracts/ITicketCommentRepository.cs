using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ITicketCommentRepository
    {
        Task<IEnumerable<CommentResponseDTO>> GetByticketIdAsync(int ticketId);
        public Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId);

        Task CreateAsync(TicketComment comment);
        Task DeleteAsync(int ticketId);
    }
}
