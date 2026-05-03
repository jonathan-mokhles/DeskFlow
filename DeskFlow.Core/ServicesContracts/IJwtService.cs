using Azure.Core;
using DeskFlow.Core.Domain.IdentityEntity;
using DeskFlow.Core.DTOs.AccountDTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DeskFlow.Core.ServicesContracts
{
    public  interface IJwtService
    {
        public Task<AuthResponseDTO> GenerateToken(ApplicationUser user,IList<string> roles);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
