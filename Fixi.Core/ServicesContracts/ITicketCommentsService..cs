using Fixi.Core.DTOs.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface ITicketCommentsService
    {
        public Task AddCommentToTicketAsync(CommentCreateDTO comment);
        public Task DeleteCommentFromTicketAsync(int commentId);

        public Task<IEnumerable<CommentResponseDTO>> GetCommentsForTicketAsync(int ticketId);
    }
}
