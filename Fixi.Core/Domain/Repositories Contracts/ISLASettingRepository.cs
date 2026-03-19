using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.SLADTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface ISLASettingRepository
    {
        Task<SLASetting?> GetByPriorityAsync(int Priority);
        Task<SLASetting?> GetByIdAsync(int Id);
        Task<IEnumerable<SLASetting>> GetAllAsync();
        Task<int> CreateAsync(SLASetting slaSetting);
        Task UpdateAsync(SLASetting slaSetting);
        Task DeleteAsync(int ticketId);
    }
}
