using DeskFkow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DeskFkow.Tests.IntegrationTest
{
    public class TicketsControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TicketsControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTicket_WithInvalidPayload_ReturnsBadRequest()
        {
            _client.SetUserToken();

            var response = await _client.PostAsJsonAsync("/api/tickets", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTicket_AsNonAdmin_ReturnsForbidden()
        {
            _client.SetUserToken();

            var response = await _client.DeleteAsync("/api/tickets/1");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
