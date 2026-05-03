using DeskFlow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;

namespace DeskFlow.Tests.IntegrationTest
{
    public class TicketCommentControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TicketCommentControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetComments_Unauthenticated_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/tickets/1/comments");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetComments_AuthenticatedForMissingTicket_ReturnsNotFound()
        {
            _client.SetUserToken();

            var response = await _client.GetAsync("/api/tickets/99999/comments");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
