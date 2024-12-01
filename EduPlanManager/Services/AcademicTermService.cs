using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class AcademicTermService : IAcademicTermService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcademicTermService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<AcademicTerm>> GetAllAcademicTermsAsync()
        {
            return await _unitOfWork.AcademicTerms.GetAllAsync();
        }
    }
}
