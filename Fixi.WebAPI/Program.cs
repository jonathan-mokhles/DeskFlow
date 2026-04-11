using Fixi.Core.Authorization.Handlers;
using Fixi.Core.Authorization.Requirements;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Domain.Repositories_Contracts;
using Fixi.Core.Services;
using Fixi.Core.ServicesContracts;
using Fixi.Infrastructure.DbContext;
using Fixi.Infrastructure.Repositories;
using Fixi.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


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
         options.AddPolicy("ManagerOrTechnician", policy => policy.RequireRole("Manager", "Technician"));

         options.AddPolicy("ManagerOrReporterOrAssignedTo", policy =>
             policy.Requirements.Add(new ManagerOrReporterOrAssignedToRequirement()));

         options.AddPolicy("ManagerOrAdmin", policy =>
             policy.Requirements.Add(new ManagerOrAdminRequirement()));
     });

builder.Services.AddScoped<IAuthorizationHandler,ManagerOrReporterOrAssignedToHandler >();
builder.Services.AddScoped<IAuthorizationHandler,ManagerOrAdminHandler >();


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ISLAService, SLAService>();
builder.Services.AddScoped<ITicketCommentsService, TicketCommentsService>();


builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketAuditLogRepository, TicketAuditLogRepository>();
builder.Services.AddScoped<ISLASettingRepository, SLASettingRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository >();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRopository>();
builder.Services.AddScoped<ITicketCommentRepository, CommentRepository>();




builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSwaggerGen(options =>
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "api.xml");
    options.IncludeXmlComments(xmlPath);
}
);

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
