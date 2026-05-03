using Microsoft.AspNetCore.Authorization;

namespace DeskFkow.Core.Authorization.Requirements
{
    public class ManagerOrTechnicianRequirement : IAuthorizationRequirement
    {
        public ManagerOrTechnicianRequirement() { }
    }
}
