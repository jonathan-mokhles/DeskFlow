using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.UsersDTOs;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Fixi.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IIdentityService _identityService;
        public UserService(IIdentityService identityService)
        {
           _identityService = identityService;
        }


        public async Task RegisterUserAsync(RegisterUserDTO registerDTO)
        {
            var existingUser = await _identityService.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                throw new ValidationException($"A user with the email '{registerDTO.Email}' already exists.");
            }

            if (!await _identityService.RoleExistsAsync(registerDTO.Role))
            {
                throw new ValidationException("Specified role does not exist.");
            }

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                DepartmentId = registerDTO.DepartmentId,
                PhoneNumber = registerDTO.Phone,

            };
            var result = await _identityService.CreateUserAsync(newUser, registerDTO.Password);

            if (!result.Succeeded)
            {
                throw new ValidationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _identityService.AddToRoleAsync(newUser, registerDTO.Role);
        }

        public async Task<IdentityResult> UpdateUserAsync(string Id, UpdateUserDTO updateDTO)
        {
            ApplicationUser? userToUpdate = await _identityService.FindByIdAsync(Id);

            if (userToUpdate == null)
            {
                throw new NotFoundException("User not found.");
            }

            userToUpdate.FullName = updateDTO.FullName;
            userToUpdate.DepartmentId = updateDTO.DepartmentId;
            userToUpdate.PhoneNumber = updateDTO.Phone;
            userToUpdate.Email = updateDTO.Email;

            await _identityService.RemoveFromRolesAsync(userToUpdate, await _identityService.GetUserRolesAsync(userToUpdate));
            if (!await _identityService.RoleExistsAsync(updateDTO.Role))
            {
                throw new ValidationException("Specified role does not exist.");
            }
            await _identityService.AddToRoleAsync(userToUpdate, updateDTO.Role);
            return await _identityService.UpdateUserAsync(userToUpdate);
        }

        public async Task<IdentityResult> DeleteUserAsync(string Id)
        {
            ApplicationUser? userToDelete = await _identityService.FindByIdAsync(Id);
            if (userToDelete == null)
            {
                throw new NotFoundException("User not found.");
            }
            userToDelete.IsActive = false;
            return await _identityService.UpdateUserAsync(userToDelete);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(string Id)
        {
            ApplicationUser? user = await _identityService.FindByIdAsync(Id);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            var roles = await _identityService.GetUserRolesAsync(user);

            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department!.Name,
                Phone = user.PhoneNumber!,
                Email = user.Email!,
                Role = roles.FirstOrDefault()!,
            };

        }

        public async Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query)
        {
            var queryable = _identityService.GetUsersQueryable();

            if (!string.IsNullOrEmpty(query.Name))
            {
                queryable = queryable.Where(u => u.FullName.Contains(query.Name));
            }
            if (query.DepartmentId.HasValue)
            {
                queryable = queryable.Where(u => u.DepartmentId == query.DepartmentId.Value);
            }

            query.PageSize = Math.Min(query.PageSize, 100);
            int skip = (query.PageNumber - 1) * query.PageSize;


            var users = await queryable.Include(u => u.Department).Skip(skip).Take(query.PageSize).ToListAsync();
            var userResponseList = new List<UserResponseDTO>();
            foreach (var user in users)
            {
                var roles = await _identityService.GetUserRolesAsync(user);
                userResponseList.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department!.Name,
                    Phone = user.PhoneNumber!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault()!,
                });
            }
            return userResponseList;
        }

    }
}
