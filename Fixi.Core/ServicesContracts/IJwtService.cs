using Azure.Core;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.DTOs.AccountDTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Fixi.Core.ServicesContracts
{
    public  interface IJwtService
    {
        public Task<AuthResponseDTO> GenerateToken(ApplicationUser user,IList<string> roles);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
