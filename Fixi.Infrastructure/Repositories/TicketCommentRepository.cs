using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.CommentDTOs;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Infrastructure.Repositories
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        ApplicationDbContext _db;

        public TicketCommentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommentResponseDTO?> GetCommentByIdAsync(int commentId)
        {
            return await _db.TicketComments.Where(c => c.Id == commentId)
                .Select(c => new CommentResponseDTO
                {
                    Id = c.Id,
                    TicketId = c.TicketId,
                    UserName = c.User.UserName!,
                    CommentText = c.CommentText,
                    IsInternal = c.IsInternal,
                    CreatedDate = c.CreatedDate,
                    UserID = c.UserId
                }).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(TicketComment comment)
        {
            await _db.AddAsync(comment);
        }

        public async Task DeleteAsync(int CommentId)
        {
            _db.TicketComments.Remove(new TicketComment { Id = CommentId});
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<CommentResponseDTO>> GetByticketIdAsync(int ticketId)
        {
            return await  _db.TicketComments.Where(c => c.TicketId == ticketId)
                .Select(c => new CommentResponseDTO
                {
                    Id = c.Id,
                    TicketId = c.TicketId,
                    UserName = c.User.UserName!,
                    CommentText = c.CommentText,
                    IsInternal = c.IsInternal,
                    CreatedDate = c.CreatedDate,
                    UserID = c.UserId
                }).ToListAsync();
        }
    }
}
