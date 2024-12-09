using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IGradeRepository : IRepositoryBase<Grade, Guid>
    {
        Task<List<Grade>> GetGradeByUserID(Guid userId);
    }
}
