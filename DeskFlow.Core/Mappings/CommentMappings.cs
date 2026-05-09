using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.CommentDTOs;

namespace DeskFlow.Core.Mappings
{
    public static class CommentMappings
    {
        public static TicketComment ToEntity(this CommentCreateDTO dto)
        {
            return new TicketComment
            {
                TicketId = dto.TicketId,
                CommentText = dto.CommentText,
                IsInternal = dto.IsInternal,
                CreatedDate = DateTime.UtcNow,
            };
        }

        public static CommentResponseDTO ToResponseDto(this TicketComment comment)
        {
            return new CommentResponseDTO
            {
                Id = comment.Id,
                TicketId = comment.TicketId,
                UserID = comment.UserId,
                UserName = comment.User?.FullName ?? string.Empty,
                CommentText = comment.CommentText,
                IsInternal = comment.IsInternal,
                CreatedDate = comment.CreatedDate
            };
        }
    }
}
