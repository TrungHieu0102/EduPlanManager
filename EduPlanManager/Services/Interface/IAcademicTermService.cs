using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IAcademicTermService
    {
        Task<IEnumerable<AcademicTerm>> GetAllAcademicTermsAsync();

    }
}
