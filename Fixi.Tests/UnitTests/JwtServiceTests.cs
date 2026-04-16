using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Exceptions;
using Fixi.Core.Services;
using Fixi.Core.ServicesContracts;
using Fixi.Core.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Security.Claims;

namespace Fixi.Tests.UnitTests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly Mock<IIdentityService> _identityService;
        public JwtServiceTests()
        {
            _identityService = new Mock<IIdentityService>();

            var jwtSettings = Options.Create(new JwtSettings
            {
                SecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
                TokenDurationInMinutes = 30,
                RefreshTokenDurationInMinutes = 8
            });

            _jwtService = new JwtService(jwtSettings, _identityService.Object);

        }

        #region GenerateToken

        [Fact]
        public async Task GenerateToken_WithValidArguments_ReturnsCompleteAuthResponse()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-id-123",
                Email = "test@example.com",
                DepartmentId = 1,
                FullName = "Test User"
            };
            var roles = new List<string> { "Admin" };


            // Act
            var result = await _jwtService.GenerateToken(user, roles);

            // Assert - All properties in one test
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("test@example.com");
            result.FullName.Should().Be("Test User");
            result.Role.Should().Be("Admin");
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.Expiration.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5));
            result.RefreshTokenExpiration.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(8), TimeSpan.FromSeconds(5));

            // Verify refresh token was saved
            _identityService.Verify(x => x.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task GenerateToken_WithNullUser_ThrowsArgumentNullException()
        {
            // Arrange
            ApplicationUser user = null;
            var roles = new List<string> { "Admin" };
            // Act & Assert
            Func<Task> act = async () => await _jwtService.GenerateToken(user, roles);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateToken_WithEmptyUserEmail_ThrowsArgumentException()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-id-123",
                Email = "", // Empty email
                DepartmentId = 1,
                FullName = "Test User"
            };
            var roles = new List<string> { "Admin" };

            // Act & Assert
            Func<Task> act = async () => await _jwtService.GenerateToken(user, roles);

            await act.Should().ThrowAsync<ArgumentNullException>();
                

        }


        #endregion


        #region GetPrincipalFromExpiredToken
        [Fact]
        public async Task GetPrincipalFromExpiredToken_WithValidToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-id-123",
                Email = "test@example.com",
                DepartmentId = 1,
                FullName = "Test User"
            };
            var roles = new List<string> { "Admin" };
            var token = await _jwtService.GenerateToken(user, roles);

            //Act
            var principal = _jwtService.GetPrincipalFromExpiredToken(token.Token);

            // Assert
            principal.Should().NotBeNull();
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
            principal.Claims.Should().Contain(c => c.Type == "DeptId" && c.Value == "1");
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_WithInvalidToken_SecurityTokenMalformedException()
        {
            // Arrange
            string Token = "InvalidTokenString";

            // Assert

            var act = () => _jwtService.GetPrincipalFromExpiredToken(Token);
            act.Should().Throw<SecurityTokenMalformedException>();
        }
        #endregion

    }
}


