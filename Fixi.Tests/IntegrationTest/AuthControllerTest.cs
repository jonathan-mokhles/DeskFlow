using Fixi.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Fixi.Tests.IntegrationTest
{
    public class AuthControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithInvalidPayload_ReturnsBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ResetPassword_Unauthenticated_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/reset-password", new
            {
                oldPassword = "old",
                newPassword = "new",
                confirmPassword = "new"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
