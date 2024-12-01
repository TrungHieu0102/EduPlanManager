namespace EduPlanManager.Models.Entities
{
    public class Class
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public string Code { get; set; }
        // Số lượng học sinh
        public int StudentCount { get; set; }

        // Số lượng giáo viên
        public int TeacherCount { get; set; }
        // Mối quan hệ nhiều-nhiều giữa Class và User
        public ICollection<User> Users { get; set; }

        // Mối quan hệ nhiều-nhiều giữa Class và Subject
        public ICollection<Subject> Subjects { get; set; }
       
    }
}
