
using System.Linq.Expressions;


namespace EduPlanManager.Repositories.Interface
{
    public interface IRepositoryBase<T, Key> where T : class
    {
        Task<T> GetByIdAsync(Key id);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAllAsync();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        IQueryable<T> FindAsync(Expression<Func<T, bool>> expression);
    }
}
