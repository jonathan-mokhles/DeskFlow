using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.DTOs.shared;
using Fixi.Core.DTOs.UsersDTOs;
using Fixi.Core.Exceptions;
using Fixi.Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Fixi.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
           _identityService = identityService;
           _unitOfWork = unitOfWork;
        }


        public async Task RegisterUserAsync(RegisterUserDTO registerDTO)
        {
            var existingUser = await _identityService.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                throw new ValidationException("Email already exists.");
            }

            if (!await _identityService.RoleExistsAsync(registerDTO.Role))
            {
                throw new ValidationException("Specified role does not exist");
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
                throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            result = await _identityService.AddToRoleAsync(newUser, registerDTO.Role);

            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

        }

        public async Task<IdentityResult> UpdateUserAsync(string Id, UpdateUserDTO updateDTO)
        {
            ApplicationUser? userToUpdate = await _identityService.FindByIdAsync(Id);

            if (userToUpdate == null)
            {
                throw new NotFoundException("User not found");
            }

            userToUpdate.FullName = updateDTO.FullName;
            userToUpdate.DepartmentId = updateDTO.DepartmentId;
            userToUpdate.PhoneNumber = updateDTO.Phone;
            var oldRoles = await _identityService.GetUserRolesAsync(userToUpdate);

            if(oldRoles.FirstOrDefault() != updateDTO.Role)
            {
                if (!await _identityService.RoleExistsAsync(updateDTO.Role))
                {
                    throw new ValidationException("Specified role does not exist");
                }
                await _identityService.RemoveFromRolesAsync(userToUpdate, oldRoles.First());
                await _identityService.AddToRoleAsync(userToUpdate, updateDTO.Role);
            }

            return await _identityService.UpdateUserAsync(userToUpdate);
        }

        public async Task<IdentityResult> DeactivateUserAsync(string Id)
        {
            ApplicationUser? userToDelete = await _identityService.FindByIdAsync(Id);
            if (userToDelete == null)
            {
                throw new NotFoundException("User not found");
            }
            userToDelete.IsActive = false;
            return await _identityService.UpdateUserAsync(userToDelete);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(string Id)
        {
            var user =  await _unitOfWork.User.GetUserByIdAsync(Id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            return user;
        }

        public async Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query)
        {
            return await _unitOfWork.User.GetAllUsersAsync(query);
        }
    }

    }



