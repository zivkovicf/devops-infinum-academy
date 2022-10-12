using AutoBogus;
using IA.DevOps.Movies.Contracts;
using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Contracts.Repositories;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using Moq;

namespace IA.DevOps.Movies.Services.Tests
{
    public abstract class BaseTests
    {
        protected Movie Movie = AutoFaker.Generate<Movie>();
        protected List<Movie> MoviesList = new();

        protected Mock<IMoviesRepository> MockMoviesRepository = new();
        protected Mock<IUnitOfWork> MockUnitOfWork = new();
        protected Mock<IMovieBlobStorageRepository> MockMovieBlobStorageRepository = new();

        protected BaseTests()
        {
            MockMoviesRepository
                .Setup(x => x.GetAsync(Movie.Id))
                .ReturnsAsync(Movie);

            MockMoviesRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(MoviesList);

            MockUnitOfWork
                .Setup(x => x.Movies)
                .Returns(MockMoviesRepository.Object);
        }
    }
}
