using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.CommentDTOs;
using DeskFlow.Core.DTOs.shared;
using DeskFlow.Core.Enums;
using DeskFlow.Core.Mappings;
using DeskFlow.Core.ServicesContracts;

namespace DeskFlow.Core.Services
{
    public class TicketCommentsService : ITicketCommentsService
    {
        IUnitOfWork _unitOfWork;
        ICurrentUserService _currentUserService;

        public TicketCommentsService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
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
            bool AllComments = true;
            if(_currentUserService.Role == RoleEnum.User )
            {
                    AllComments = false;
            }
            return _unitOfWork.TicketComment.GetByticketIdAsync(ticketId, AllComments);
        }
    }
}
