using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        Task SaveAsync();
    }
}
