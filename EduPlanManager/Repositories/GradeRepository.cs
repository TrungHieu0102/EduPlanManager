using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class GradeRepository(ApplicationDbContext context) : RepositoryBase<Grade, Guid>(context), IGradeRepository
    {
        public async Task<List<Grade>> GetGradeByUserID(Guid userId)
        {
            var grades = await _context.Grades
                .Include(s => s.Subject)
                .Include(a => a.AcademicTerm)
                .Where(g => g.StudentId == userId)
                .ToListAsync();
            return grades;
        }
    }
}
