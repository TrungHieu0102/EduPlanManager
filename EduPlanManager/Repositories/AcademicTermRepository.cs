using EduPlanManager.Data;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.Repositories
{
    public class AcademicTermRepository(ApplicationDbContext context) : RepositoryBase<AcademicTerm, Guid>(context), IAcademicTermRepository
    {
    }
}
