using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface ITicketCommentRepository
    {
        Task<IEnumerable<CommentResponseDTO>> GetByticketIdAsync(int ticketId);
        public Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId);

        Task CreateAsync(TicketComment comment);
        Task DeleteAsync(int ticketId);
    }
}
