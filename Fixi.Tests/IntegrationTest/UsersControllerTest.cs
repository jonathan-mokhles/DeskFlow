using DeskFkow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DeskFkow.Tests.IntegrationTest
{
    public class UsersControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UsersControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_Unauthenticated_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/users", new
            {
                fullName = "Test User",
                departmentId = 1,
                phone = "1234567890",
                email = "test.user@example.com",
                password = "Password123!",
                confirmPassword = "Password123!",
                role = "User"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_AsNonAdmin_ReturnsForbidden()
        {
            _client.SetManagerToken();

            var response = await _client.PostAsJsonAsync("/api/users", new
            {
                fullName = "Test User",
                departmentId = 1,
                phone = "1234567890",
                email = "test.user@example.com",
                password = "Password123!",
                confirmPassword = "Password123!",
                role = "User"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
