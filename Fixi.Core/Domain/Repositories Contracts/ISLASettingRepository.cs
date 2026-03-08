using Fixi.Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ISLASettingRepository
    {
        Task<SLASetting?> GetByPriorityAsync(int Priority);
        Task<IEnumerable<SLASetting>> GetAllAsync();
        Task<SLASetting> CreateAsync(SLASetting slaSetting);
        Task UpdateAsync(SLASetting slaSetting);
        Task DeleteAsync(int ticketId);
    }
}
