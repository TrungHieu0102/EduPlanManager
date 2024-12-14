using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class SumaryGradeRepository(ApplicationDbContext context) : RepositoryBase<SumaryGrade, Guid>(context), ISumaryGradeRepository
    {
        public async Task<SumaryGrade?> GetSummaryGrade(Guid studentId, Guid subjectId, Guid academicTermId)
        {
            return await _context.SumaryGrades
                .FirstOrDefaultAsync(sg => sg.StudentId == studentId
                                           && sg.SubjectId == subjectId
                                           && sg.AcademicTermId == academicTermId);
        }

    }
}
