using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IGradeRepository : IRepositoryBase<Grade, Guid>
    {
        Task<List<Grade>> GetGradeByUserID(Guid userId);
        Task<List<Grade>> GetStudentGradeOnTeacher(List<Class> teacherClasses, List<Subject> teacherSubjects);    
    }
}
