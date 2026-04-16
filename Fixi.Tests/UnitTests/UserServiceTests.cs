using Fixi.Core.Services;
using Fixi.Core.ServicesContracts;
using Moq;
using Fixi.Core.DTOs.UsersDTOs;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.Domain.IdentityEntity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using FluentAssertions;
using Fixi.Core.Exceptions;

namespace Fixi.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IIdentityService> _identityService;
        private readonly Mock<IUserRepository> _userRepository;

        public UserServiceTests()
        {
            _identityService = new Mock<IIdentityService>();
            _userRepository = new Mock<IUserRepository>();
            _userService = new UserService(_identityService.Object, _userRepository.Object);
        }


        #region RegisterUserAsync

        [Fact]
        public async Task RegisterUserAsync_WithExistingEmail_ThrowsValidationException()
        {
            // Arrange
            var registerDTO = new RegisterUserDTO
            {
                Email = "existEmail",
            };
            _identityService.Setup(x => x.FindByEmailAsync(registerDTO.Email)).ReturnsAsync(new ApplicationUser { Email = registerDTO.Email });

            // Act & Assert
            var act =() =>  _userService.RegisterUserAsync(registerDTO);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Email already exists.");
        }

        [Fact]
        public async Task RegisterUserAsync_WithNonExistingRole_ThrowsValidationException()
        {
            // Arrange
            var registerDTO = new RegisterUserDTO
            {
                Email = "newEmail",
                Role = "NonExistingRole"
            };

            _identityService.Setup(x => x.FindByEmailAsync(registerDTO.Email)).ReturnsAsync((ApplicationUser)null);
            _identityService.Setup(x => x.RoleExistsAsync(registerDTO.Role)).ReturnsAsync(false);

            // Act & Assert
            // Act & Assert
            var act = () => _userService.RegisterUserAsync(registerDTO);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Specified role does not exist");
        }

        [Fact]
        public async Task RegisterUserAsync_WhenCreateUserFails_ThrowsValidationException()
        {
            // Arrange
            var registerDTO = new RegisterUserDTO
            {
                Email = "NewEmail",
                Role = "ExistingRole"
            };

            _identityService.Setup(x => x.FindByEmailAsync(registerDTO.Email)).ReturnsAsync((ApplicationUser)null);
            _identityService.Setup(x => x.RoleExistsAsync(registerDTO.Role)).ReturnsAsync(true);
            _identityService.Setup(x=> x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerDTO.Role))
                .ReturnsAsync(IdentityResult.Success);
            _identityService.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>(), registerDTO.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to create user" }));
            _identityService.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), registerDTO.Role)).ReturnsAsync(IdentityResult.Success);


            // Act & Assert
            var act = () => _userService.RegisterUserAsync(registerDTO);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Failed to create user");
        }

        [Fact]
        public async Task RegisterUserAsync_WhenAddToRoleFails_ThrowsValidationException()
        {
            // Arrange
            var registerDTO = new RegisterUserDTO
            {
                Email = "NewEmail",
                Role = "ExistingRole"
            };
            _identityService.Setup(x => x.FindByEmailAsync(registerDTO.Email)).ReturnsAsync((ApplicationUser)null);
            _identityService.Setup(x => x.RoleExistsAsync(registerDTO.Role)).ReturnsAsync(true);
            _identityService.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerDTO.Role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            // Act & Assert
            var act = () => _userService.RegisterUserAsync(registerDTO);
            await act.Should().ThrowAsync<ValidationException>();
        }
        [Fact]
        public async Task RegisterUserAsync_WithValidData_CreatesUserAndAssignsRole()
        {
            // Arrange
            var registerDTO = new RegisterUserDTO
            {
                Email = "NewEmail",
                Role = "ExistingRole"

            };
            _identityService.Setup(x => x.FindByEmailAsync(registerDTO.Email)).ReturnsAsync((ApplicationUser)null);
            _identityService.Setup(x => x.RoleExistsAsync(registerDTO.Role)).ReturnsAsync(true);
            _identityService.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>(), registerDTO.Password)).ReturnsAsync(IdentityResult.Success);
            _identityService.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerDTO.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
             var act = () => _userService.RegisterUserAsync(registerDTO);

            await act.Should().NotThrowAsync();
            _identityService.Verify(x => x.CreateUserAsync(It.IsAny<ApplicationUser>(), registerDTO.Password), Times.Once);
            _identityService.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerDTO.Role), Times.Once);
        }


        #endregion


        #region UpdateUserAsync

        [Fact]
        public async Task UpdateUserAsync_WithNonExistingUser_ThrowsValidationException()
        {
            // Arrange
            var updateDTO = new UpdateUserDTO
            {
                Email = "nonExistingEmail",
                Role = "ExistingRole"
            };
            _identityService.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            // Act & Assert
            var act = () => _userService.UpdateUserAsync("nonExistingId", updateDTO);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistingRole_ThrowsValidationException()
        {
            // Arrange
            var updateDTO = new UpdateUserDTO
            {
                Email = "existingEmail",
                Role = "NonExistingRole"
            };
            _identityService.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Email = "existingEmail" });
            _identityService.Setup(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new[] {"Role"});
            _identityService.Setup(x => x.RoleExistsAsync(updateDTO.Role)).ReturnsAsync(false);
            // Act & Assert
            var act = () => _userService.UpdateUserAsync("existingId", updateDTO);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Specified role does not exist");
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidDataNewRole_UpdatesUserAndRole()
        {
            // Arrange
            var updateDTO = new UpdateUserDTO
            {
                Email = "existingEmail",
                Role = "ExistingRole"
            };
            var existingUser = new ApplicationUser { Email = "existingEmail" };
            _identityService.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(existingUser);
            _identityService.Setup(x => x.RoleExistsAsync(updateDTO.Role)).ReturnsAsync(true);
            _identityService.Setup(x => x.UpdateUserAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            _identityService.Setup(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _identityService.Setup(x => x.AddToRoleAsync(existingUser, updateDTO.Role)).ReturnsAsync(IdentityResult.Success);
            _identityService.Setup(x => x.GetUserRolesAsync(existingUser)).ReturnsAsync(new List<string> { "OldRole" });
            // Act
            var act = () => _userService.UpdateUserAsync("existingId", updateDTO);
            await act.Should().NotThrowAsync();
            _identityService.Verify(x => x.UpdateUserAsync(existingUser), Times.Once);
            _identityService.Verify(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<string>()), Times.Once);
            _identityService.Verify(x => x.AddToRoleAsync(existingUser, updateDTO.Role), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidDataSameRole_UpdatesUserAndRole()
        {
            // Arrange
            var updateDTO = new UpdateUserDTO
            {
                Email = "existingEmail",
                Role = "ExistingRole"
            };
            var existingUser = new ApplicationUser { Email = "existingEmail" };
            _identityService.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(existingUser);
            _identityService.Setup(x => x.GetUserRolesAsync(existingUser)).ReturnsAsync(new List<string> { "ExistingRole" });
            _identityService.Setup(x => x.UpdateUserAsync(existingUser)).ReturnsAsync(IdentityResult.Success);
            // Act
            var act = () => _userService.UpdateUserAsync("existingId", updateDTO);
            await act.Should().NotThrowAsync();
            _identityService.Verify(x => x.UpdateUserAsync(existingUser), Times.Once);
            _identityService.Verify(x => x.RemoveFromRolesAsync(existingUser, It.IsAny<string>()), Times.Never);
            _identityService.Verify(x => x.AddToRoleAsync(existingUser, updateDTO.Role), Times.Never);
        }

        #endregion


        #region DeactivateUserAsync
        [Fact]
        public async Task DeactivateUserAsync_NonExistingUser_ThrowNotFoundException()
        {
            ApplicationUser user = new ApplicationUser 
            {
                Id = "Id" 
            };
             _identityService.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync((ApplicationUser)null);

            Func<Task> act = async () => await _userService.DeactivateUserAsync(user.Id);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task DeactivateUserAsync_ExistingUser_DeactivatesUser()
        {
            ApplicationUser user = new ApplicationUser 
            {
                Id = "Id" 
            };
            _identityService.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _identityService.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(IdentityResult.Success);
            Func<Task> act = async () => await _userService.DeactivateUserAsync(user.Id);
            await act.Should().NotThrowAsync();
            _identityService.Verify(x => x.UpdateUserAsync(It.Is<ApplicationUser>(u => u.Id == user.Id && u.IsActive == false)), Times.Once);
        }


        #endregion


        #region GetUserByIdAsync

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ThrowNotFoundException()
        {
            string userId = "nonExistingId";
            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((UserResponseDTO)null);
            Func<Task> act = async () => await _userService.GetUserByIdAsync(userId);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            string userId = "existingId";
            UserResponseDTO user = new UserResponseDTO { Id = userId, Email = "exsitEmail" };
            _userRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            var result = await _userService.GetUserByIdAsync(userId);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(user);
        }
        #endregion

        }
}
