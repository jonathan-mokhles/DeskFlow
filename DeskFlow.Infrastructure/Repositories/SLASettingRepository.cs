using System;
using System.Collections.Generic;
using System.Text;
using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.SLADTOs;
using DeskFkow.Core.Enums;
using DeskFkow.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DeskFkow.Infrastructure.Repositories
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
            return slaSetting.Id;
        }

        public Task DeleteAsync(int id)
        {
            _db.SLASettings.Remove(new SLASetting { Id = id });
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
            return Task.CompletedTask;
        }
    }
}
