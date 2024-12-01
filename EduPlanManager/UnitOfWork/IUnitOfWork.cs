using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ISubjectRepository Subjects { get; }
        Task<int> CompleteAsync();
        void Dispose();

    }
}
