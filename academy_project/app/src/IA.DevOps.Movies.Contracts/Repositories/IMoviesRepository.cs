using IA.DevOps.Movies.Contracts.Entities;

namespace IA.DevOps.Movies.Contracts.Repositories
{
    public interface IMoviesRepository : IBaseRepository<Movie, Guid>
    {
    }
}
