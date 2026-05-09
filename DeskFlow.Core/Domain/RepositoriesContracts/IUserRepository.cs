using System;
using System.Collections.Generic;
using System.Text;
using DeskFlow.Core.DTOs.UsersDTOs;

namespace DeskFlow.Core.Domain.RepositoriesContracts
{
    public interface IUserRepository
    {
        public Task<UserResponseDTO?> GetUserByIdAsync(string Id);
        public Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters query);
        Task<string?> GetManagerDepartmentEmailByCategoryIdAsync(int categoryId);
    }
}
