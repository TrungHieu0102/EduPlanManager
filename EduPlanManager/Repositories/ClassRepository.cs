using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class ClassRepository(ApplicationDbContext context) : RepositoryBase<Class, Guid>(context), IClassRepository
    {
        public async Task<bool> CheckExists(string name, string code)
        {
            var existingClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassName == name || c.Code == code);
            return existingClass == null;
        }
        public IQueryable<Class> GetQueryable()
        {
            return _context.Classes.AsQueryable();
        }
        public async Task<int> GetTotalClassesAsync(string searchTerm)
        {
            IQueryable<Class> query = _context.Classes;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Code.Contains(searchTerm) || s.ClassName.Contains(searchTerm));
            }        

            return await query.CountAsync();
        }
        public async Task<List<Class>> GetClassesByIdsAsync(List<Guid> ids)
        {
            return await _context.Classes.Where(s => ids.Contains(s.Id)).ToListAsync();
        }
        public async Task DeleteClassesAsync(List<Class> classes)
        {
            _context.Classes.RemoveRange(classes);
            await _context.SaveChangesAsync();
        }
        public async Task<Class?> GetClassUserAsync(Guid classId)
        {
            return await _context.Classes.Include(c => c.Users)
                                      .FirstOrDefaultAsync(c => c.Id == classId);
        }
        public async Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        }

        public async Task<Class?> GetClassSubjectAsync(Guid classId)
        {
            return await _context.Classes.Include(c => c.Subjects)
                                     .FirstOrDefaultAsync(c => c.Id == classId);
        }

      
    }
}
