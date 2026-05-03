using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.SLADTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Domain.RepositoriesContracts
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
