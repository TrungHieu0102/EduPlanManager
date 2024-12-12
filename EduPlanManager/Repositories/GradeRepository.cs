using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public async Task<List<Grade>> GetStudentGradeOnTeacher(List<Class> teacherClasses, List<Subject> teacherSubjects)
        {
            var teacherClassIds = teacherClasses.Select(c => c.Id).ToList();
            var teacherSubjectIds = teacherSubjects.Select(s => s.Id).ToList();

            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Include(g => g.Student.Classes)
                .Where(g => teacherSubjectIds.Contains(g.SubjectId) &&
                            g.Student.Classes.Any(sc => teacherClassIds.Contains(sc.Id)))
                .ToListAsync();
        }


    }
}

