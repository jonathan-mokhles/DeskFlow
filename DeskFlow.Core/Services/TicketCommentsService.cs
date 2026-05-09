using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.CommentDTOs;
using DeskFlow.Core.Mappings;
using DeskFlow.Core.ServicesContracts;

namespace DeskFlow.Core.Services
{
    public class TicketCommentsService : ITicketCommentsService
    {
        IUnitOfWork _unitOfWork;

        public TicketCommentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task AddCommentToTicketAsync(TicketComment comment)
        {
            await _unitOfWork.TicketComment.CreateAsync(comment);
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
