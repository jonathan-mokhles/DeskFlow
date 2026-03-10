using Fixi.Core.Services;
using Microsoft.AspNetCore.Identity;
using Fixi.Core.Domain.IdentityEntity;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using AutoFixture;

namespace Fixi.Tests.UnitTests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IConfiguration> _configuration;

        public JwtServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Configuration setup
            _configuration = new Mock<IConfiguration>();
            SetupConfiguration();

            _jwtService = new JwtService(_configuration.Object, _userManager.Object);
        }
        private void SetupConfiguration()
        {
            _configuration.Setup(x => x["Jwt:SecretKey"])
                .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
            _configuration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _configuration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
            _configuration.Setup(x => x["Jwt:DurationInMinutes"]).Returns("30");
        }

        [Fact]
        public async Task GenerateToken_WithValidUser_ReturnsCompleteAuthResponse()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-id-123",
                Email = "test@example.com",
                FullName = "Test User"
            };
            var roles = new List<string> { "Admin" };
            _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _jwtService.GenerateToken(user);

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
            _userManager.Verify(x => x.UpdateAsync(user), Times.Once);
        }
    }
}
