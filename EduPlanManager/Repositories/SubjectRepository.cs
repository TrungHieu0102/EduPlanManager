using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class SubjectRepository(ApplicationDbContext context) : RepositoryBase<Subject, Guid>(context), ISubjectRepository
    {
       

        public async Task<Subject> GetSubjectWithDetailsAsync(Guid id)
        {
            var subject = await _context.Subjects
                .Include(s => s.AcademicTerm)  
                .Include(s => s.Category)     
                .FirstOrDefaultAsync(s => s.Id == id)?? throw new Exception("Subject not found"); 
            return subject;
        }
        public async Task<int> GetTotalSubjectsAsync(string searchTerm, int? semester, int? year)
        {
            IQueryable<Subject> query = _context.Subjects.Include(s => s.Category).Include(s => s.AcademicTerm);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Code.Contains(searchTerm) || s.Name.Contains(searchTerm));
            }

            if (semester.HasValue)
            {
                query = query.Where(s => s.AcademicTerm.Semester == semester);
            }

            if (year.HasValue)
            {
                query = query.Where(s => s.AcademicTerm.Year == year);
            }

            return await query.CountAsync();
        }
        public IQueryable<Subject> GetQueryable()
        {
            return _context.Subjects
                  .Include(s => s.Category)        
                  .Include(s => s.AcademicTerm)     
                  .AsQueryable();
        }
        public async Task<List<Subject>> GetSubjectsByIdsAsync(List<Guid> ids)
        {
            return await _context.Subjects.Where(s => ids.Contains(s.Id)).ToListAsync();
        }

        public async Task DeleteSubjectsAsync(List<Subject> subjects)
        {
            _context.Subjects.RemoveRange(subjects);
            await _context.SaveChangesAsync();
        }

    }
}
