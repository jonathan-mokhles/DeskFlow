using DeskFkow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;

namespace DeskFkow.Tests.IntegrationTest
{
    public class SLAControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SLAControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllSLA_Unauthenticated_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/sla");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllSLA_AsNonAdmin_ReturnsForbidden()
        {
            _client.SetTechnicianToken();

            var response = await _client.GetAsync("/api/sla");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
