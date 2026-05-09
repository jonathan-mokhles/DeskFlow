using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.DTOs.UsersDTOs;
using DeskFlow.Core.Enums;
using DeskFlow.WebAPI.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DeskFlow.WebAPI.Authorization.Handlers
{
    public class AdminOrManagerOrUserHandler : AuthorizationHandler<AdminOrManagerOrUserRequirement, UserResponseDTO>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrManagerOrUserRequirement requirement, UserResponseDTO resource)
        {
            string role = context.User.FindFirstValue(ClaimTypes.Role)!;
            string userid = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            int deptId = int.Parse(context.User.FindFirstValue("DeptId")!);

            if (role == nameof(RoleEnum.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            if (role == nameof(RoleEnum.Manager) && deptId == resource.DepartmentId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            if (role == nameof(RoleEnum.User) && userid == resource.Id)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}
