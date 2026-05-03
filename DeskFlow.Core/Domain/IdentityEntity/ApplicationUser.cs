using DeskFkow.Core.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeskFkow.Core.Domain.IdentityEntity
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

    }
}
