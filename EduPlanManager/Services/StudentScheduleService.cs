using EduPlanManager.Models.DTOs.Schedule;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class StudentScheduleService : IStudentScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentScheduleService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<WeeklyScheduleDto>> GetWeeklySchedulesAsync(Guid studentId)
        {
            var studentSchedules = await _unitOfWork.StudentSchedules.GetStudentSchedulesAsync(studentId);

            if (!studentSchedules.Any())
                return new List<WeeklyScheduleDto>();
            var academicTerm = studentSchedules.FirstOrDefault()?.AcademicTerm;

            if (academicTerm == null)
                return new List<WeeklyScheduleDto>();
            var weeklySchedules = new List<WeeklyScheduleDto>();
            var startDate = academicTerm.StartDate;
            var endDate = academicTerm.EndDate;

            for (var date = startDate; date <= endDate; date = date.AddDays(7))
            {
                var weekSchedule = new WeeklyScheduleDto
                {
                    WeekStartDate = date,
                    WeekEndDate = date.AddDays(6)
                };

                foreach (var schedule in studentSchedules)
                {
                    var dayOfWeek = Enum.Parse<DayOfWeek>(schedule.DayOfWeek, true);

                    if (date <= schedule.AcademicTerm.EndDate &&
                        date.AddDays(6) >= schedule.AcademicTerm.StartDate)
                    {
                        weekSchedule.DailySchedules.Add(new DailyScheduleDto
                        {
                            DayOfWeek = dayOfWeek,
                            SubjectName = schedule.Subject.Name,
                            StartTime = schedule.StartTime,
                            EndTime = schedule.EndTime
                        });
                    }
                }

                weeklySchedules.Add(weekSchedule);
            }

            return weeklySchedules;
        }

    }
}
