using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface ITicketCommentRepository
    {
        Task<IEnumerable<CommentResponseDTO>> GetByticketIdAsync(int ticketId);
        public Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId);

        Task CreateAsync(TicketComment comment);
        Task DeleteAsync(int ticketId);
    }
}
