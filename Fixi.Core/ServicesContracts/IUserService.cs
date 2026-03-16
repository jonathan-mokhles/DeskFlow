using Fixi.Core.DTOs.UsersDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public interface IUserService
    {
        public Task RegisterUserAsync(RegisterUserDTO registerDTO);
        public Task<IdentityResult> UpdateUserAsync(string Id,UpdateUserDTO updateDTO);

        public Task<IdentityResult> DeleteUserAsync(string Id);

        //public Task<ApplicationUser> GetUserByIdAsync(string Id);
    }
}
