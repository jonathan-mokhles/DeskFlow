using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.DTOs.UsersDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.ServicesContracts
{
    public interface IIdentityService
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, string role);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    }
}
