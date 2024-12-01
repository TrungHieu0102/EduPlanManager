using EduPlanManager.Data;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EduPlanManager.Repositories
{
    public abstract class RepositoryBase<T, Key> : IRepositoryBase<T, Key> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public async Task<T> GetByIdAsync(Key id)
        {
            return await _dbSet.FindAsync(id) ?? throw new InvalidOperationException("Entity not found");
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public IQueryable<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
    }
}
