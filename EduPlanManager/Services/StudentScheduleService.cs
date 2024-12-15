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

        /// <summary>
        /// Lấy lịch học hàng tuần của sinh viên theo ID sinh viên.
        /// Phương thức này sẽ lấy danh sách lịch học của sinh viên từ cơ sở dữ liệu, sau đó nhóm lịch học theo tuần. 
        /// Mỗi tuần sẽ bao gồm các ngày học và các môn học trong ngày. 
        /// Nếu không có lịch học cho sinh viên hoặc không có học kỳ tương ứng, phương thức sẽ trả về danh sách trống.
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <returns>Danh sách các lịch học theo tuần của sinh viên dưới dạng WeeklyScheduleDto</returns>
        public async Task<List<WeeklyScheduleDto>> GetWeeklySchedulesAsync(Guid studentId)
        {
            // Lấy danh sách lịch học của sinh viên từ cơ sở dữ liệu
            var studentSchedules = await _unitOfWork.StudentSchedules.GetStudentSchedulesAsync(studentId);

            // Kiểm tra nếu không có lịch học nào, trả về danh sách trống
            if (!studentSchedules.Any())
                return new List<WeeklyScheduleDto>();

            // Lấy học kỳ của sinh viên từ lịch học đầu tiên
            var academicTerm = studentSchedules.FirstOrDefault()?.AcademicTerm;

            // Nếu không có học kỳ, trả về danh sách trống
            if (academicTerm == null)
                return new List<WeeklyScheduleDto>();

            var weeklySchedules = new List<WeeklyScheduleDto>();
            var startDate = academicTerm.StartDate;
            var endDate = academicTerm.EndDate;

            // Lặp qua từng tuần trong học kỳ
            for (var date = startDate; date <= endDate; date = date.AddDays(7))
            {
                // Tạo lịch học tuần mới
                var weekSchedule = new WeeklyScheduleDto
                {
                    WeekStartDate = date,
                    WeekEndDate = date.AddDays(6)
                };

                // Lặp qua tất cả các lịch học của sinh viên
                foreach (var schedule in studentSchedules)
                {
                    // Chuyển đổi ngày trong tuần từ chuỗi sang kiểu Enum
                    var dayOfWeek = Enum.Parse<DayOfWeek>(schedule.DayOfWeek, true);

                    // Kiểm tra nếu ngày trong tuần nằm trong phạm vi của tuần hiện tại
                    if (date <= schedule.AcademicTerm.EndDate && date.AddDays(6) >= schedule.AcademicTerm.StartDate)
                    {
                        // Thêm lịch học hàng ngày vào lịch học tuần
                        weekSchedule.DailySchedules.Add(new DailyScheduleDto
                        {
                            DayOfWeek = dayOfWeek,
                            SubjectName = schedule.Subject.Name,
                            StartTime = schedule.StartTime,
                            EndTime = schedule.EndTime
                        });
                    }
                }

                // Thêm lịch học tuần vào danh sách lịch học của sinh viên
                weeklySchedules.Add(weekSchedule);
            }

            // Trả về danh sách các lịch học hàng tuần của sinh viên
            return weeklySchedules;
        }


    }
}
