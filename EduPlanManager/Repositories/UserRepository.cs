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
            return await _context.Users.Include(u => u.Classes).ToListAsync();

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
        public async Task<List<User>> GetUsersWithoutClassAsync(Guid classId)
        {
            return await _context.Users
                                 .Where(u => !u.Classes.Any(c => c.Id == classId))
                                 .ToListAsync();
        }
        public async Task<List<User>> GetUserssByIdsAsync(List<Guid> ids)
        {
            return await _context.Users.Where(s => ids.Contains(s.Id)).ToListAsync();
        }
        public async Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Include(u => u.Classes) 
                .ToListAsync();
        }
        public async Task<List<Class>> GetClassesByUserIdsAsync(List<Guid> userIds)
        {
            return await _context.Classes
                .Where(c => c.Users.Any(u => userIds.Contains(u.Id))) 
                .Include(c => c.Users)
                .ToListAsync();
        }
        public async Task RemoveUsersAsync(List<User> users)
        {
            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync();
        }

       
    }
}
