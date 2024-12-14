using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface ISumaryGradeRepository :  IRepositoryBase<SumaryGrade, Guid>
    {
        Task<SumaryGrade?> GetSummaryGrade(Guid studentId, Guid subjectId, Guid academicTermId);
    }
}
