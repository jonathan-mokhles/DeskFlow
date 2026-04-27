using Fixi.Core.Authorization.Requirements;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fixi.Core.Authorization.Handlers
{
    public class ManagerOrReporterOrAssignedToHandler : AuthorizationHandler<ManagerOrReporterOrAssignedToRequirement, TicketDTO>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerOrReporterOrAssignedToRequirement requirement, TicketDTO resource)
        {
            string userId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            string Role = context.User.FindFirstValue(ClaimTypes.Role)!;
            int deptId = int.Parse(context.User.FindFirstValue("DeptId")!);

            if (resource.AssignedToId == userId || resource.ReportedById == userId)
            {
                context.Succeed(requirement);
            }

            if (Role == nameof(RoleEnum.Manager) && deptId == resource.DepartmentId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
