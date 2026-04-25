using Fixi.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace Fixi.Tests.IntegrationTest.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");


            builder.ConfigureServices(services =>
            {
                var toRemove = services.Where(d =>
                    d.ServiceType.FullName != null &&
                    d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                    d.ServiceType == typeof(ApplicationDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions)
                ).ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            });
        }

        }

    }

