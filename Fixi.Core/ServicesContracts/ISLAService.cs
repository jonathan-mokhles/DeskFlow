using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.SLADTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface ISLAService
    {
        public Task<int> CreateSLA(SLACreateDTO createDTO);
        public Task DeleteSLA(int id);
        public Task<IEnumerable<SLASetting>> GetAllSLA();
        public Task<SLASetting?> GetSLAById(int id);
        public Task UpdateSLA(SLASetting setting);

    }
}
