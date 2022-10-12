using IA.DevOps.Movies.Contracts.Entities;
using System.Linq.Expressions;

namespace IA.DevOps.Movies.Contracts.Repositories
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : Base
    {
        Task<TEntity?> GetAsync(TKey id);
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);

        Task SaveChangesAsync();
    }
}
