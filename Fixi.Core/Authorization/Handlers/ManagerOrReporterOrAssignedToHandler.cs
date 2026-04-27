using Fixi.Core.Authorization.Requirements;
using Fixi.Core.Domain.Repositories_Contracts;
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
    public class ManagerOrReporterOrAssignedToHandler : AuthorizationHandler<ManagerOrReporterOrAssignedToRequirement, int>
    {
        private readonly ITicketRepository _ticketRepository;
        public ManagerOrReporterOrAssignedToHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManagerOrReporterOrAssignedToRequirement requirement, int resource)
        {
            var ticket = await _ticketRepository.GetTicketAsync(resource);
            if (ticket == null)
            {
                return;
            }

            string userId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            string Role = context.User.FindFirstValue(ClaimTypes.Role)!;
            int deptId = int.Parse(context.User.FindFirstValue("DeptId")!);

            if (ticket.AssignedToId == userId || ticket.ReportedById == userId)
            {
                context.Succeed(requirement);
            }

            if (Role == nameof(RoleEnum.Manager) && deptId == ticket.DepartmentId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
