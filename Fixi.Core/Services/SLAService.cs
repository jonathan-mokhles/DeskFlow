using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Text;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.SLADTOs;
using DeskFkow.Core.Mappings;

namespace DeskFkow.Core.Services
{
    public  class SLAService : ISLAService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SLAService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<int> CreateSLA(SLACreateDTO createDTO)
        {
            var setting = createDTO.ToEntity();

            await _unitOfWork.SLASetting.CreateAsync(setting);
            await _unitOfWork.CommitAsync();
            return setting.Id;
        }

        public async Task DeleteSLA(int id)
        {
            await _unitOfWork.SLASetting.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<SLASetting>> GetAllSLA()
        {
            return await _unitOfWork.SLASetting.GetAllAsync();
        }

        public Task<SLASetting?> GetSLAById(int id)
        {
            return _unitOfWork.SLASetting.GetByIdAsync(id);
        }

        public async Task UpdateSLA(SLASetting setting)
        {
            await _unitOfWork.SLASetting.UpdateAsync(setting);
            await _unitOfWork.CommitAsync();
        }
    }
}
