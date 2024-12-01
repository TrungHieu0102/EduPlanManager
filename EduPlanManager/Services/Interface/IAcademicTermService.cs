using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IAcademicTermService
    {
        Task<IEnumerable<AcademicTerm>> GetAllAcademicTermsAsync();
        Task<Result<AcademicTermDTO>> CreateAcademicTermsAsync(CreateUpdateAcademicTermDTO createAcademicTerm);
        Task<Result<bool>> DeleteAcademicAsync(Guid id);
        Task<Result<AcademicTermDTO>> UpdateAcademic(CreateUpdateAcademicTermDTO request);
    }
}
