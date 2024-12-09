using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class StudentScheduleRepository(ApplicationDbContext context) : RepositoryBase<StudentSchedule, Guid>(context), IStudentScheduleRepository
    {
        public async Task AddStudentScheduleAsync(StudentSchedule studentSchedule)
        {
            await _context.StudentSchedules.AddAsync(studentSchedule);
        }

        public async Task<List<StudentSchedule>> GetStudentSchedulesAsync(Guid studentId)
        {
            return await _context.StudentSchedules
                    .Include(s => s.Subject)
                    .Include(s => s.AcademicTerm)
                    .Where(s => s.StudentId == studentId)
                    .ToListAsync();
        }
    }
}
