using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.Repositories
{
    public class SubjectCategoryRepository(ApplicationDbContext context) : RepositoryBase<SubjectCategory, Guid>(context), ISubjectCategoryRepository
    {
    }
}
