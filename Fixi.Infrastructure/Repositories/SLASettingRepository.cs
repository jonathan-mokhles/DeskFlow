using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.SLADTOs;
using Fixi.Core.Enums;
using Fixi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Fixi.Infrastructure.Repositories
{
    public class SLASettingRepository : ISLASettingRepository
    {
        ApplicationDbContext _db;

        public SLASettingRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<int> CreateAsync(SLASetting slaSetting)
        {
            _db.SLASettings.Add(slaSetting);
            await _db.SaveChangesAsync();
            return slaSetting.Id;
        }

        public Task DeleteAsync(int id)
        {
            _db.SLASettings.Remove(new SLASetting { Id = id });
            _db.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<SLASetting>> GetAllAsync()
        {
            return await _db.SLASettings.ToListAsync();
        }

        public Task<SLASetting?> GetByIdAsync(int Id)
        {
            return _db.SLASettings.FirstOrDefaultAsync(s => s.Id == Id);    
        }

        public async Task<SLASetting?> GetByPriorityAsync(int Priority)
        {
            SLASetting? setting = await _db.SLASettings.FirstOrDefaultAsync(s => (int)s.Priority == Priority);

            return setting;
        }

        public Task UpdateAsync(SLASetting slaSetting)
        {
            _db.SLASettings.Update(slaSetting);
            _db.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
