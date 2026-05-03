using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DeskFlow.Core.Domain.IdentityEntity;

namespace DeskFlow.Core.Domain.Entity
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ManagerId { get; set; }


        [ForeignKey("ManagerId")]
        public ApplicationUser? Manager { get; set; }
    }
}
