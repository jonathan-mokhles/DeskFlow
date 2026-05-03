using Azure.Core;
using DeskFkow.Core.Domain.IdentityEntity;
using DeskFkow.Core.DTOs.AccountDTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DeskFkow.Core.ServicesContracts
{
    public  interface IJwtService
    {
        public Task<AuthResponseDTO> GenerateToken(ApplicationUser user,IList<string> roles);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
