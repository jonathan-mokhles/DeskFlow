using DeskFkow.Core.Domain.Entity;
using DeskFkow.Core.DTOs.SLADTOs;

namespace DeskFkow.Core.Mappings
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
