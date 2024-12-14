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
                .FirstOrDefaultAsync(s => s.Id == id) ?? throw new Exception("Subject not found");
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
        public async Task<List<Subject>> GetSubjectsClassAsync(bool isHaveClass, Guid classId)
        {
            return await _context.Subjects
                                 .Where(u => isHaveClass ? u.Classes.Any(c => c.Id == classId) : !u.Classes.Any(c => c.Id == classId))
                                 .ToListAsync();
        }
        public async Task<List<Subject>> GetSubjectsScheduleAsync(bool isHaveSchedule, Guid scheduleId)
        {
            return await _context.Subjects
                                 .Where(u => isHaveSchedule ? u.Schedules.Any(c => c.Id == scheduleId) : !u.Schedules.Any(c => c.Id == scheduleId))
                                 .ToListAsync();
        }

        public async Task<bool> IsSubjectExistsInformation(string code, Guid academicTermId, Guid teacherId)
        {
            var exists = await _context.Subjects.AnyAsync(s =>
                s.Code == code &&
                s.AcademicTermId == academicTermId &&
                s.TeacherId == teacherId);

            return exists;
        }
        public async Task<List<Subject>> GetAllSubjectByUserId(Guid userId)
        {
            return await _context.Subjects.Include(t => t.Teacher)
                                        .Include(c => c.Classes)
                                        .Where(s => s.TeacherId == userId)
                                        .ToListAsync();
        }
    }
}
