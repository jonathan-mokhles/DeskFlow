using DeskFkow.Core.Authorization.Requirements;
using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.TicketDTOs;
using DeskFkow.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFkow.Core.Authorization.Handlers
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
