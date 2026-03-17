using Microsoft.AspNetCore.Authorization;

namespace Fixi.Core.Authorization.Requirements
{
    public class DepartmentManagerOrAdminRequirement : IAuthorizationRequirement
    {
        public DepartmentManagerOrAdminRequirement() { }
    }
}
