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
