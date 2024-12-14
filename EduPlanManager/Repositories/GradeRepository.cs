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
        public async Task<List<Grade>> GetAllGrade()
        {
            var grades = await _context.Grades
                .Include(s => s.Subject)
                .Include(a => a.AcademicTerm)
                .Include(sd => sd.Student)
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
        public async Task<bool> CheckUsersExitsGrade(Guid userId, GradeType type, Guid subjectId, Guid academicTermId)
        {
            return await _context.Grades.AnyAsync(g => g.StudentId == userId &&g.Type == type && g.SubjectId == subjectId && g.AcademicTermId == academicTermId);
        }
        public async Task<Grade?> GetGradeWithoutId(Guid userId, GradeType type,  Guid subjectId, Guid academicTermId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Include(g => g.AcademicTerm)
                .FirstOrDefaultAsync(g => g.StudentId == userId && g.Type== type && g.SubjectId == subjectId && g.AcademicTermId == academicTermId);
        }
        public async Task<List<Grade>> GetGradesBySubjectAndStudent(Guid studentId, Guid subjectId, Guid academicTermId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId
                            && g.SubjectId == subjectId
                            && g.AcademicTermId == academicTermId)
                .ToListAsync();
        }


    }
}

