using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly BankingDbContext _context;

        public RoleRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(
                    r => r.RoleName == roleName);
        }
    }
}