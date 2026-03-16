using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.UsersDTOs;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task RegisterUserAsync(RegisterUserDTO registerDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                throw new ValidationException($"A user with the email '{registerDTO.Email}' already exists.");
            }

            if (!await _roleManager.RoleExistsAsync(registerDTO.Role))
            {
                throw new ValidationException("Specified role does not exist.");
            }

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                DepartmentId = registerDTO.DepartmentId,
                PhoneNumber = registerDTO.phone,

            };
            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (!result.Succeeded)
            {
                throw new ValidationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(newUser, registerDTO.Role);
        }

        public Task<IdentityResult> UpdateUserAsync(string Id, UpdateUserDTO updateDTO)
        {
            ApplicationUser? userToUpdate = _userManager.Users.FirstOrDefault(u => u.Id == updateDTO.Id);

            if (userToUpdate == null)
            {
                throw new NotFoundException("User not found.");
            }

            userToUpdate.FullName = updateDTO.FullName;
            userToUpdate.DepartmentId = updateDTO.DepartmentId;
            userToUpdate.PhoneNumber = updateDTO.phone;
            userToUpdate.Email = updateDTO.Email;

            return _userManager.UpdateAsync(userToUpdate);

        }

        public async Task<IdentityResult> DeleteUserAsync(string Id)
        {
            ApplicationUser? userToDelete = _userManager.Users.FirstOrDefault(u => u.Id == Id);
            if (userToDelete == null)
            {
                throw new NotFoundException("User not found.");
            }
            userToDelete.IsActive = false;
            return await _userManager.UpdateAsync(userToDelete);
        }
    }
}
