using Fixi.Core.Domain.Entity;
using Fixi.Core.DTOs.SLADTOs;

namespace Fixi.Core.Mappings
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
