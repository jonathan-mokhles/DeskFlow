using Fixi.Core.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Core.Domain.IdentityEntity
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }

        // Navigation property
        public Department? Department { get; set; }
    }
}
