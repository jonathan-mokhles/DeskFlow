using Fixi.Core.Authorization.Requirements;
using Fixi.Core.DTOs.TicketDTOs;
using Fixi.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Authorization.Handlers
{
    public class ManagerOrAdminHandler : AuthorizationHandler<ManagerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerOrAdminRequirement requirement, int resource)
        {
            string role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value!;
            int deptID = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            if ((role == nameof(RoleEnum.Manager) && deptID == resource) || role == nameof(RoleEnum.Admin))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
