using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CommentDTOs;
using Fixi.Core.ServicesContracts;
using Fixi.Core.Domain.Entity;

namespace Fixi.Core.Services
{
    public class TicketCommentsService : ITicketCommentsService
    {
        ITicketCommentRepository _repo;

        public TicketCommentsService(ITicketCommentRepository repo)
        {
            _repo = repo;
        }


        public async Task AddCommentToTicketAsync(CommentCreateDTO comment)
        {
            await _repo.CreateAsync(new TicketComment { 
                CommentText = comment.CommentText,
                TicketId = comment.TicketId,
                CreatedDate = comment.CreatedDate,
                IsInternal = comment.IsInternal,
                UserId = comment.UserId,

            });
        }

        public Task DeleteCommentFromTicketAsync(int commentId)
        {
            return _repo.DeleteAsync(commentId);
        }

        public Task<IEnumerable<CommentResponseDTO>> GetCommentsForTicketAsync(int ticketId)
        {
            return _repo.GetByticketIdAsync(ticketId);
        }
    }
}
