using DeskFlow.Core.DTOs.shared;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DeskFlow.WebAPI
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        public string Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "";

        public int DeptId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst("DeptId")?.Value;
                return int.TryParse(value, out var id) ? id : 0;
            }
        }
    }
}
