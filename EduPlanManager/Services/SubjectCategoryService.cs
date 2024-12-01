using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class SubjectCategoryService : ISubjectCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubjectCategory>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.SubjectCategories.GetAllAsync();
        }
    }
}
