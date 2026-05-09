using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public interface ITicketCommentsService
    {
        public Task AddCommentToTicketAsync(TicketComment comment);
        public Task DeleteCommentFromTicketAsync(int commentId);

        public Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId);

        public Task<IEnumerable<CommentResponseDTO>> GetCommentsForTicketAsync(int ticketId);
    }
}
