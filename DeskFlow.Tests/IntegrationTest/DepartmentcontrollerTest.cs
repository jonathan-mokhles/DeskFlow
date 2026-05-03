using DeskFkow.Core.Domain.Entity;
using DeskFkow.Infrastructure.DbContext;
using DeskFkow.Tests.IntegrationTest.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DeskFkow.Tests.IntegrationTest
{
    public class DepartmentcontrollerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public DepartmentcontrollerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private void SeedDepartment(string name)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Departments.Add(new Department { Name = name });
            db.SaveChanges();
        }

        #region GetAllDepartments
        [Fact]
        public async Task GetAllDepartments_AsAdmin_ReturnsOkWithDepartments()
        {
            SeedDepartment("IT Support");
            _client.SetAdminToken();

            var response = await _client.GetAsync("/api/department");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var departments = await response.Content.ReadFromJsonAsync<List<Department>>();
            departments.Should().NotBeNull();
            departments!.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllDepartments_AsNonAdmin_ReturnsForbidden()
        {
            _client.SetUserToken();
            var response = await _client.GetAsync("/api/department");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region AddDepartment
        [Fact]
        public async Task AddDepartment_AsAdmin_ReturnsCreated()
        {
            _client.SetAdminToken();

            var response = await _client.PostAsync("/api/department?name=Finance", null);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<Department>();
            created!.Name.Should().Be("Finance");
        }
        #endregion

    }
}
