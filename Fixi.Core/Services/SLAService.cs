using Fixi.Core.Domain.Entity;
using Fixi.Core.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.SLADTOs;

namespace Fixi.Core.Services
{
    public  class SLAService : ISLAService
    {
        private readonly ISLASettingRepository _slaRepository;

        public SLAService(ISLASettingRepository slaRepository)
        {
            _slaRepository = slaRepository;
        }


        public Task<int> CreateSLA(SLACreateDTO createDTO)
        {
            return _slaRepository.CreateAsync(new SLASetting 
            { 
                Priority = createDTO.Priority,
                ResolutionTimeMinutes = createDTO.ResolutionTimeMinutes,
                ResponseTimeMinutes = createDTO.ResponseTimeMinutes
            });
        }

        public Task DeleteSLA(int id)
        {
            return _slaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<SLASetting>> GetAllSLA()
        {
            return await _slaRepository.GetAllAsync();
        }

        public Task<SLASetting?> GetSLAById(int id)
        {
            return _slaRepository.GetByIdAsync(id);
        }

        public Task UpdateSLA(SLASetting setting)
        {
            return _slaRepository.UpdateAsync(setting);
        }
    }
}
