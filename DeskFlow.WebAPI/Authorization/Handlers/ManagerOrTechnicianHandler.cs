using DeskFlow.Core.Authorization.Requirements;
using DeskFlow.Core.Domain.RepositoriesContracts;
using DeskFlow.Core.DTOs.TicketDTOs;
using DeskFlow.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Authorization.Handlers
{
    public class ManagerOrTechnicianHandler : AuthorizationHandler<ManagerOrTechnicianRequirement, int>
    {
        private readonly ITicketRepository _ticketRepository;
        public ManagerOrTechnicianHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerOrTechnicianRequirement requirement, int resource)
        {
            var ticket = await _ticketRepository.GetTicketAsync(resource);
            if (ticket == null)
            {
                context.Fail();
                return;
            }
            string role = context.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value!;
            int deptID = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value ?? "0");
            if ((role == nameof(RoleEnum.Manager)|| role == nameof(RoleEnum.Technician)) && deptID == ticket.DepartmentId)
            {
                context.Succeed(requirement);
            }
            return;
        }
    }
}
