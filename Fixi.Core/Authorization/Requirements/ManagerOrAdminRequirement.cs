using Microsoft.AspNetCore.Authorization;

namespace Fixi.Core.Authorization.Requirements
{
    public class ManagerOrAdminRequirement : IAuthorizationRequirement
    {
        public ManagerOrAdminRequirement() { }
    }
}
