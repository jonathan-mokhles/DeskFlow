using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Authorization.Requirements
{
    public class ManagerOrReporterOrAssignedToRequirement : IAuthorizationRequirement
    {
        public ManagerOrReporterOrAssignedToRequirement() { }
    }
}
