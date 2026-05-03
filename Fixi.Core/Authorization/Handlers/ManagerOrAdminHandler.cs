using Fixi.Core.Authorization.Requirements;
using Fixi.Core.Domain.Repositories_Contracts;
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
        private readonly ITicketRepository _ticketRepository;
        public ManagerOrAdminHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerOrAdminRequirement requirement, int resource)
        {
            var ticket = await _ticketRepository.GetTicketAsync(resource);
            if (ticket == null)
            {
                context.Fail();
                return;
            }
            string role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value!;
            int deptID = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            if ((role == nameof(RoleEnum.Manager) && deptID == ticket.DepartmentId) || role == nameof(RoleEnum.Admin))
            {
                context.Succeed(requirement);
            }
            return;
        }
    }
}
