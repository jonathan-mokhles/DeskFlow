using DeskFlow.Core.DTOs.UsersDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public interface IUserService
    {
        public Task RegisterUserAsync(RegisterUserDTO registerDTO);
        public Task<IdentityResult> UpdateUserAsync(string Id,UpdateUserDTO updateDTO);

        public Task<IdentityResult> DeactivateUserAsync(string Id);

        public Task<UserResponseDTO> GetUserByIdAsync(string Id);

        public Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query);
    }
}
