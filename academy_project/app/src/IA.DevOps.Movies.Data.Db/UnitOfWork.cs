using IA.DevOps.Movies.Contracts;
using IA.DevOps.Movies.Contracts.Repositories;
using IA.DevOps.Movies.Data.Db.Repositories;

namespace IA.DevOps.Movies.Data.Db
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MoviesDbContext _dbContext;
        private IMoviesRepository _moviesRepository = default!;

        public IMoviesRepository Movies
        {
            get
            {
                return _moviesRepository ??= new MoviesRepository(_dbContext);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public UnitOfWork(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
