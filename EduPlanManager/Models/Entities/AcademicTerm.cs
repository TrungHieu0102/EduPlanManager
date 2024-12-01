namespace EduPlanManager.Models.Entities
{
    public class AcademicTerm
    {
        public Guid Id { get; set; } // Khóa chính
        public int Year { get; set; } // Năm học
        public int Semester { get; set; } // Học kỳ: 1, 2, hoặc 3
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc

        public ICollection<Subject> Subjects { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<StudentSchedule> Schedules { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}
