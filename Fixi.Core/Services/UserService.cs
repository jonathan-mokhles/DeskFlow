using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.DTOs.UsersDTOs;
using DeskFkow.Core.Exceptions;
using DeskFkow.Core.Mappings;
using DeskFkow.Core.ServicesContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace DeskFkow.Core.Services
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

            ApplicationUser newUser = registerDTO.ToEntity();

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

            updateDTO.ApplyTo(userToUpdate);
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



