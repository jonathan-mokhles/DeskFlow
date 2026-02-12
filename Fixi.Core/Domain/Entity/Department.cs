using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Fixi.Core.Domain.IdentityEntity;

namespace Fixi.Core.Domain.Entity
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
