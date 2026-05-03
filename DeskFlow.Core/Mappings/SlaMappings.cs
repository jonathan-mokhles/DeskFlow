using DeskFlow.Core.Domain.Entity;
using DeskFlow.Core.DTOs.SLADTOs;

namespace DeskFlow.Core.Mappings
{
    public static class SlaMappings
    {
        public static SLASetting ToEntity(this SLACreateDTO dto)
        {
            return new SLASetting
            {
                Priority = dto.Priority,
                ResponseTimeMinutes = dto.ResponseTimeMinutes,
                ResolutionTimeMinutes = dto.ResolutionTimeMinutes
            };
        }
    }
}
