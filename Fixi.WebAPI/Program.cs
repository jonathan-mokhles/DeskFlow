using DeskFkow.Core.Authorization.Handlers;
using DeskFkow.Core.Authorization.Requirements;
using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.shared;
using DeskFkow.Core.Enums;
using DeskFkow.Core.Services;
using DeskFkow.Core.ServicesContracts;
using DeskFkow.Core.Settings;
using DeskFkow.Infrastructure.DbContext;
using DeskFkow.Infrastructure.Repositories;
using DeskFkow.WebAPI;
using DeskFkow.WebAPI.Middlewares;
using Hangfire;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))// from Secrets Manager
    };
});

builder.Services.AddAuthorization( options =>
     {
         options.AddPolicy("AdminOrManager", policy => policy.RequireRole(nameof(RoleEnum.Manager), nameof(RoleEnum.Admin)));
         options.AddPolicy("AdminOnly", policy => policy.RequireRole(nameof(RoleEnum.Admin)));

         options.AddPolicy("ManagerOrReporterOrAssignedTo", policy =>
             policy.Requirements.Add(new ManagerOrReporterOrAssignedToRequirement()));

         options.AddPolicy("ManagerOrAdmin", policy =>
             policy.Requirements.Add(new ManagerOrAdminRequirement()));
         options.AddPolicy("ReporterOnly", policy =>
             policy.Requirements.Add(new ReporterOnlyRequirement()));
         options.AddPolicy("ManagerOrAdmin", policy =>
             policy.Requirements.Add(new ManagerOrAdminRequirement()));
     });

builder.Services.AddScoped<IAuthorizationHandler,ManagerOrReporterOrAssignedToHandler >();
builder.Services.AddScoped<IAuthorizationHandler,ManagerOrAdminHandler >();
builder.Services.AddScoped<IAuthorizationHandler,ReporterOnlyHandler >();
builder.Services.AddScoped<IAuthorizationHandler,ManagerOrAdminHandler >();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ISLAService, SLAService>();
builder.Services.AddScoped<ITicketCommentsService, TicketCommentsService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ITicketAttachmentService, TicketAttachmentService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IMailService, MailService>();




builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketAuditLogRepository, TicketAuditLogRepository>();
builder.Services.AddScoped<ISLASettingRepository, SLASettingRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository >();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRopository>();
builder.Services.AddScoped<ITicketCommentRepository, TicketCommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketAttachmentRepository, TicketAttachmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();





builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var error = new ApiErrorResponse
            {
                Message = "Validation failed",
                Errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList(),
                TraceId = context.HttpContext.TraceIdentifier
            };

            return new BadRequestObjectResult(error);
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "api.xml");
    options.IncludeXmlComments(xmlPath);
}
);

// Add Hangfire services
builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();


var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseExceptionHandlingMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();


RecurringJob.AddOrUpdate<ITicketService>("CheckSLA", service => service.UpdateSLAStatusesAsync(), Cron.Hourly);

app.Run();
