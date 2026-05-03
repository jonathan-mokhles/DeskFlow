using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.CommentDTOs;

namespace DeskFkow.Core.Mappings
{
    public static class CommentMappings
    {
        public static TicketComment ToEntity(this CommentCreateDTO dto)
        {
            return new TicketComment
            {
                TicketId = dto.TicketId,
                UserId = dto.UserId,
                CommentText = dto.CommentText,
                IsInternal = dto.IsInternal,
                CreatedDate = dto.CreatedDate
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
