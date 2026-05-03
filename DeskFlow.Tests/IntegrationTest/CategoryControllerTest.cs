using DeskFlow.Core.DTOs.CategoryDTOs;
using DeskFlow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DeskFlow.Tests.IntegrationTest
{
    public class CategoryControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CategoryControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllCategories_Unauthenticated_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/category");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateCategory_AsNonAdmin_ReturnsForbidden()
        {
            _client.SetUserToken();

            var payload = new CreateCategoryDTO
            {
                Name = "Hardware",
                DepartmentId = 1,
                Description = "Hardware related requests"
            };

            var response = await _client.PostAsJsonAsync("/api/category", payload);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
