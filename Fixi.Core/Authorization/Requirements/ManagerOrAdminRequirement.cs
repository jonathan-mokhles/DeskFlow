using Microsoft.AspNetCore.Authorization;

namespace DeskFkow.Core.Authorization.Requirements
{
    public class ManagerOrAdminRequirement : IAuthorizationRequirement
    {
        public ManagerOrAdminRequirement() { }
    }
}
