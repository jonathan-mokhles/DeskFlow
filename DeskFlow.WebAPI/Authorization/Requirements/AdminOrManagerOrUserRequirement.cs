using Microsoft.AspNetCore.Authorization;

namespace DeskFlow.WebAPI.Authorization.Requirements
{
    public class AdminOrManagerOrUserRequirement : IAuthorizationRequirement
    {
    }
}
