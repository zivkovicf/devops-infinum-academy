using IA.DevOps.Movies.Contracts.Repositories;

namespace IA.DevOps.Movies.Contracts
{
    public interface IUnitOfWork
    {
        IMoviesRepository Movies { get; }

        Task SaveChangesAsync();
    }
}
