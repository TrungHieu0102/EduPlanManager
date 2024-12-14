using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IGradeRepository : IRepositoryBase<Grade, Guid>
    {
        Task<List<Grade>> GetGradeByUserID(Guid userId);
        Task<List<Grade>> GetAllGrade();
        Task<List<Grade>> GetStudentGradeOnTeacher(List<Class> teacherClasses, List<Subject> teacherSubjects);
        Task<bool> CheckUsersExitsGrade(Guid userId, GradeType type, Guid subjectId, Guid academicTermId);
        Task<Grade> GetGradeWithoutId(Guid userId, GradeType type, Guid subjectId, Guid academicTermId);
        Task<List<Grade>> GetGradesBySubjectAndStudent(Guid studentId, Guid subjectId, Guid academicTermId);
    }
}
