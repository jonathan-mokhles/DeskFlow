using Microsoft.AspNetCore.Authorization;

namespace DeskFlow.Core.Authorization.Requirements
{
    public class ManagerOrTechnicianRequirement : IAuthorizationRequirement
    {
        public ManagerOrTechnicianRequirement() { }
    }
}
