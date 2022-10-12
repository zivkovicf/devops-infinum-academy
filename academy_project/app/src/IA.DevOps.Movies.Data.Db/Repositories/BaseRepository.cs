using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IA.DevOps.Movies.Data.Db.Repositories
{
    public abstract class BaseRepository<TContext, TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TContext : DbContext
        where TEntity : Base
    {
        private readonly DbSet<TEntity> _dbSet;

        private readonly TContext _dbContext;

        protected BaseRepository(TContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = QueryWithIncludes(includes);

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = QueryWithIncludes(includes);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = QueryWithIncludes(includes);

            return await query.Where(predicate).ToListAsync();
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        protected IQueryable<TEntity> QueryWithIncludes(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            return includes.Aggregate(query, (query_part, include) => query_part.Include(include));
        }
    }
}
