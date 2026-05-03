using DeskFlow.Core.Authorization.Requirements;
using DeskFlow.Core.Domain.RepositoriesContracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DeskFlow.Core.Authorization.Handlers
{
    public class ReporterOnlyHandler : AuthorizationHandler<ReporterOnlyRequirement, int>
    {
        private readonly ITicketRepository _ticketRepository;

        public ReporterOnlyHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ReporterOnlyRequirement requirement, int resource)
        {
            var ticket = await _ticketRepository.GetTicketAsync(resource);
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (ticket != null && ticket.ReportedById == userId)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
