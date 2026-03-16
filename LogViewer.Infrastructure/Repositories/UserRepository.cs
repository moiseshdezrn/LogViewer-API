using LogViewer.Core.Entities;
using LogViewer.Infrastructure.DatabaseContext;
using LogViewer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LogViewer.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, LogDbContext>, IUserRepository
    {
        public UserRepository(LogDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }
    }
}
