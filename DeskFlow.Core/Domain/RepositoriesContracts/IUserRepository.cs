using System;
using System.Collections.Generic;
using System.Text;
using DeskFkow.Core.DTOs.UsersDTOs;

namespace DeskFkow.Core.Domain.RepositoriesContracts
{
    public interface IUserRepository
    {
        public Task<UserResponseDTO?> GetUserByIdAsync(string Id);
        public Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query);
    }
}
