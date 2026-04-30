using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CommentDTOs;
using Fixi.Core.Mappings;
using Fixi.Core.ServicesContracts;

namespace Fixi.Core.Services
{
    public class TicketCommentsService : ITicketCommentsService
    {
        IUnitOfWork _unitOfWork;

        public TicketCommentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task AddCommentToTicketAsync(CommentCreateDTO comment)
        {
            await _unitOfWork.TicketComment.CreateAsync(comment.ToEntity());
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteCommentFromTicketAsync(int commentId)
        {
            await _unitOfWork.TicketComment.DeleteAsync(commentId);
            await _unitOfWork.CommitAsync();
        }

        public Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId)
        {
            return _unitOfWork.TicketComment.GetCommentByIdAsync(commentId);
        }

        public Task<IEnumerable<CommentResponseDTO>> GetCommentsForTicketAsync(int ticketId)
        {
            return _unitOfWork.TicketComment.GetByticketIdAsync(ticketId);
        }
    }
}
