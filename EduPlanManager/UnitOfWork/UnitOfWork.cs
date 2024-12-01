using EduPlanManager.Data;
using EduPlanManager.Repositories;
using EduPlanManager.Repositories.Interface;
using System.Linq.Expressions;

namespace EduPlanManager.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public ISubjectRepository Subjects { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            Subjects = new SubjectRepository(_context);
        }

       

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
       
    }
}
