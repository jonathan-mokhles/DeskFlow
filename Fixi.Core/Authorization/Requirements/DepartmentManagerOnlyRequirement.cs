using Microsoft.AspNetCore.Authorization;

namespace Fixi.Core.Authorization.Requirements
{
    public class DepartmentManagerOnlyRequirement : IAuthorizationRequirement
    {
        public DepartmentManagerOnlyRequirement() { }
    }
}
