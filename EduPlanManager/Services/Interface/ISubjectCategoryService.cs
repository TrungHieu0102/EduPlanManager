using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface ISubjectCategoryService
    {
        Task<IEnumerable<SubjectCategory>> GetAllCategoriesAsync();

    }
}
