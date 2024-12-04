using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class SubjectScheduleRepository(ApplicationDbContext context) : RepositoryBase<SubjectSchedule, Guid>(context), ISubjectScheduleRepository
    {
        public async Task DeleteAsync(SubjectSchedule schedule)
        {
            _context.SubjectSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsDuplicateScheduleAsync(int? dayOfWeek, int? session)
        {
            var query = _context.SubjectSchedules.AsQueryable();

            return await query.AnyAsync(s => (int)s.DayOfWeek == dayOfWeek && (int)s.Session == session ); 
        }
        public async Task UpdateAsync(SubjectSchedule schedule)
        {
            _context.SubjectSchedules.Update(schedule);
            await _context.SaveChangesAsync();

        }
        public async Task<SubjectSchedule?> GetScheduleSubjectAsync(Guid scheduleId)
        {
            return await _context.SubjectSchedules.Include(c => c.Subjects)
                                     .FirstOrDefaultAsync(c => c.Id == scheduleId);
        }
    }
}
