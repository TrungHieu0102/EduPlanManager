using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.Repositories
{
    public class StudentScheduleRepository(ApplicationDbContext context) : RepositoryBase<StudentSchedule, Guid>(context), IStudentScheduleRepository
    {
        public async Task AddStudentScheduleAsync(StudentSchedule studentSchedule)
        {
            await _context.StudentSchedules.AddAsync(studentSchedule);
        }

    }
}
