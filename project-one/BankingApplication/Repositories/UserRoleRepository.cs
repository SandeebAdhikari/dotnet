using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly BankingDbContext _context;

        public UserRoleRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }

        public async Task<bool> UserHasRoleAsync(
            int userId,
            string roleName)
        {
            return await (
                from userRole in _context.UserRoles
                join role in _context.Roles
                    on userRole.RoleId equals role.RoleId
                where userRole.UserId == userId
                    && role.RoleName == roleName
                select userRole
            ).AnyAsync();
        }
    }
}