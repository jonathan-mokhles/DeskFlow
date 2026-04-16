using System;
using System.Collections.Generic;
using System.Text;
using Fixi.Core.DTOs.UsersDTOs;

namespace Fixi.Core.Domain.Repositories_Contracts
{
    public interface IUserRepository
    {
        public Task<UserResponseDTO?> GetUserByIdAsync(string Id);
        public Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query);
    }
}
