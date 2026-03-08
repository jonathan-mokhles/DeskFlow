using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.Repositories_Contracts;
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


        public Task<SLASetting> CreateAsync(SLASetting slaSetting)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SLASetting>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<SLASetting?> GetByPriorityAsync(int Priority)
        {
            SLASetting? setting = await _db.SLASettings.FirstOrDefaultAsync(s => (int)s.Priority == Priority);

            return setting;
        }

        public Task UpdateAsync(SLASetting slaSetting)
        {
            throw new NotImplementedException();
        }
    }
}
