using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Contracts.Repositories;

namespace IA.DevOps.Movies.Data.Db.Repositories
{
    public class MoviesRepository : BaseRepository<MoviesDbContext, Movie, Guid>, IMoviesRepository
    {
        public MoviesRepository(MoviesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
