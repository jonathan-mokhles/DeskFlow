using Fixi.Core.Authorization.Requirements;
using Fixi.Core.DTOs.TicketDTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Authorization.Handlers
{
    public class DepartmentManagerOnlyHandler : AuthorizationHandler<DepartmentManagerOnlyRequirement, TicketDTO>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DepartmentManagerOnlyRequirement requirement, TicketDTO resource)
        {
            string role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            int deptID = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            if (role == "Manager" && deptID == resource.DepartmentId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
