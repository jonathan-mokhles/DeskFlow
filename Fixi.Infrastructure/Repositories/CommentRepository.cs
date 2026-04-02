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
    public class CommentRepository : ITicketCommentRepository
    {
        ApplicationDbContext _db;

        public CommentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(TicketComment comment)
        {
            await _db.AddAsync(comment);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int CommentId)
        {
            _db.TicketComments.Remove(new TicketComment { Id = CommentId});
            await _db.SaveChangesAsync();
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
                    CreatedDate = c.CreatedDate
                }).ToListAsync();
        }
    }
}
