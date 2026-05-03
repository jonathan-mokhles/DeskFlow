using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.DTOs.AccountDTOs;
using DeskFlow.Core.DTOs.UsersDTOs;
using System;

namespace DeskFlow.Core.Mappings
{
    public static class UserMappings
    {
        public static ApplicationUser ToEntity(this RegisterUserDTO dto)
        {
            return new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                DepartmentId = dto.DepartmentId,
                PhoneNumber = dto.Phone
            };
        }

        public static void ApplyTo(this UpdateUserDTO dto, ApplicationUser user)
        {
            user.FullName = dto.FullName;
            user.DepartmentId = dto.DepartmentId;
            user.PhoneNumber = dto.Phone;
        }

        public static UserResponseDTO ToResponseDto(this ApplicationUser user, string role, string departmentName)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                DepartmentId = user.DepartmentId,
                DepartmentName = departmentName,
                Phone = user.PhoneNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = role
            };
        }

        public static AuthResponseDTO ToAuthResponseDto(this ApplicationUser user, string role, string token, DateTime expiration, string refreshToken, DateTime refreshTokenExpiration)
        {
            return new AuthResponseDTO
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Token = token,
                Expiration = expiration,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                Role = role
            };
        }
    }
}
