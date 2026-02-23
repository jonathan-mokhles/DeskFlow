using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Fixi.Core.DTOs.AccountDTOs
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        public string OldPassword { get; set; }= string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
