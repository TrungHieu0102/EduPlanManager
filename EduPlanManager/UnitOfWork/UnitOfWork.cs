using EduPlanManager.Data;
using EduPlanManager.Repositories.Interface;

namespace EduPlanManager.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _context = context;
            Users = userRepository;
            Roles = roleRepository;
        }

        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
