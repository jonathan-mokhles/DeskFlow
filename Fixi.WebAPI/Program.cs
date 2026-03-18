using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Services;
using Fixi.Core.ServicesContracts;
using Fixi.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Infrastructure.Repositories;
using Fixi.WebAPI.Middlewares;
using Fixi.Core.Authorization.Handlers;
using Fixi.Core.Authorization.Requirements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))// Secret key from Secrets Manager
    };
});

builder.Services.AddAuthorization( options =>
     {
         options.AddPolicy("AdminOrManager", policy => policy.RequireRole("Manager","Admin"));
         options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
         options.AddPolicy("ManagerOrTechnician", policy => policy.RequireRole("Manader", "Technician"));

         options.AddPolicy("ManagerOrReporterOrAssignedTo", policy =>
             policy.Requirements.Add(new ManagerOrReporterOrAssignedToRequirement()));

         options.AddPolicy("ManagerOrAdmin", policy =>
             policy.Requirements.Add(new ManagerOrAdminRequirement()));
     });

builder.Services.AddScoped<IAuthorizationHandler,ManagerOrReporterOrAssignedToHandler >();
builder.Services.AddScoped<IAuthorizationHandler,ManagerOrAdminHandler >();


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITicketService, TicketService>();


builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketAuditLogRepository, TicketAuditLogRepository>();
builder.Services.AddScoped<ISLASettingRepository, SLASettingRepository>();




builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseExceptionHandlingMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
