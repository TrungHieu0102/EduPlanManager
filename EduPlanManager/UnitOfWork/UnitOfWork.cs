using EduPlanManager.Data;
using EduPlanManager.Repositories;
using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public ISubjectRepository Subjects { get; private set; }
        public IAcademicTermRepository AcademicTerms { get; set; }
        public ISubjectCategoryRepository SubjectCategories { get; set; }
        public IClassRepository Classes { get; set; }
        public ISubjectScheduleRepository SubjectSchedules { get; set; }
        public IEnrollmentRepository Enrollments { get; set; }
        public IStudentScheduleRepository StudentSchedules { get; set; }
        public IGradeRepository Grades { get; set; }
        public ISumaryGradeRepository SumaryGrades { get; set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            Subjects = new SubjectRepository(_context);
            AcademicTerms = new AcademicTermRepository(_context);
            SubjectCategories = new SubjectCategoryRepository(_context);
            Classes = new ClassRepository(_context);
            SubjectSchedules = new SubjectScheduleRepository(_context);
            Enrollments = new EnrollmentRepository(_context);
            StudentSchedules = new StudentScheduleRepository(_context);
            Grades = new GradeRepository(_context);
            SumaryGrades = new SumaryGradeRepository(_context);
        }

       

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
       
    }
}
