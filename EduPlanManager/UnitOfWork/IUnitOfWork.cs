using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ISubjectRepository Subjects { get; }
        IAcademicTermRepository AcademicTerms { get; }
        ISubjectCategoryRepository SubjectCategories { get; }
        IClassRepository Classes { get; }
        ISubjectScheduleRepository SubjectSchedules { get; }
        IEnrollmentRepository Enrollments { get; }
        IStudentScheduleRepository StudentSchedules { get; }
        IGradeRepository Grades { get; }
        Task<int> CompleteAsync();
        void Dispose();

    }
}
