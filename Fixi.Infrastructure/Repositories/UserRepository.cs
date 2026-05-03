using DeskFkow.Core.Domain.RepositoriesContracts;
using DeskFkow.Core.DTOs.UsersDTOs;
using Microsoft.EntityFrameworkCore;
using DeskFkow.Infrastructure.DbContext;

namespace DeskFkow.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UserResponseDTO>> GetAllUsersAsync(UsersQueryParameters param)
        {
            const int MaxPageSize = 100;
            param.PageSize = Math.Min(param.PageSize, MaxPageSize);

            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId into urj
                        from ur in urj.DefaultIfEmpty()
                        join r in _context.Roles on ur.RoleId equals r.Id into rj
                        from r in rj.DefaultIfEmpty()
                        join d in _context.Departments on u.DepartmentId equals d.Id into dj
                        from d in dj.DefaultIfEmpty()
                        select new
                        {
                            User = u,
                            DepartmentName = d != null ? d.Name : null,
                            RoleName = r != null ? r.Name : null
                        };

            if (!string.IsNullOrEmpty(param.Name))
            {
                query = query.Where(u => u.User.FullName.Contains(param.Name));
            }
            if (param.DepartmentId.HasValue)
            {
                query = query.Where(u => u.User.DepartmentId == param.DepartmentId.Value);
            }

            var page = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return page
                .GroupBy(x => x.User.Id)
                .Select(g => new UserResponseDTO
                {
                    Id = g.First().User.Id,
                    FullName = g.First().User.FullName,
                    DepartmentId = g.First().User.DepartmentId,
                    DepartmentName = g.Select(x => x.DepartmentName).FirstOrDefault() ?? "Unknown",
                    Phone = g.First().User.PhoneNumber ?? string.Empty,
                    Email = g.First().User.Email ?? string.Empty,
                    Role = g.Select(x => x.RoleName).FirstOrDefault() ?? "UnAssigned"
                })
                .ToList();
        }

        public Task<UserResponseDTO?> GetUserByIdAsync(string Id)
        {
            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId into urj
                        from ur in urj.DefaultIfEmpty()
                        join r in _context.Roles on ur.RoleId equals r.Id into rj
                        from r in rj.DefaultIfEmpty()
                        join d in _context.Departments on u.DepartmentId equals d.Id into dj
                        from d in dj.DefaultIfEmpty()
                        where u.Id == Id
                        select new UserResponseDTO
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            DepartmentId = u.DepartmentId,
                            DepartmentName = d != null ? d.Name : "Unknown",
                            Phone = u.PhoneNumber ?? string.Empty,
                            Email = u.Email ?? string.Empty,
                            Role = r != null ? r.Name : "UnAssigned"
                        };
            return query.FirstOrDefaultAsync();
        }
    }
}
