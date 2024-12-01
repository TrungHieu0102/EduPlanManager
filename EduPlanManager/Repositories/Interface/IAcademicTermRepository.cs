using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IAcademicTermRepository : IRepositoryBase<AcademicTerm, Guid>
    {
         Task<AcademicTerm> CheckExists(CreateUpdateAcademicTermDTO createAcademicTerm);
    }
}
