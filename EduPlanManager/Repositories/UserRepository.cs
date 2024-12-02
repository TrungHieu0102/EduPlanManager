using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;


namespace EduPlanManager.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<List<User>> GetUsersWithoutClassAsync()
        {
            return await _context.Users
                                   .Where(u => !u.Classes.Any()) 
                                   .ToListAsync();
        }
    }
}
