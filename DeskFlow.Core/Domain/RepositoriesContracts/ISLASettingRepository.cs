using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.SLADTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Domain.RepositoriesContracts
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
