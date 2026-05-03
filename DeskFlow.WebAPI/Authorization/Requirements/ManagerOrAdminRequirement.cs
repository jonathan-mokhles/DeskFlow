using Microsoft.AspNetCore.Authorization;

namespace DeskFlow.Core.Authorization.Requirements
{
    public class ManagerOrAdminRequirement : IAuthorizationRequirement
    {
        public ManagerOrAdminRequirement() { }
    }
}
