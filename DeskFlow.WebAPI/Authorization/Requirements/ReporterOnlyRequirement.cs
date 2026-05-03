using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeskFlow.Core.Authorization.Requirements
{
    public class ReporterOnlyRequirement : IAuthorizationRequirement
    {
        public ReporterOnlyRequirement() { }
    }
}
