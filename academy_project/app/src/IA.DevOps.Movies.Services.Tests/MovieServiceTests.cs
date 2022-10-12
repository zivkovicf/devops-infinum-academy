using FluentAssertions;
using IA.DevOps.Movies.Common.Exceptions.Types;
using IA.DevOps.Movies.Contracts.DTOs;
using Xunit;

namespace IA.DevOps.Movies.Services.Tests
{
    public class MovieServiceTests : BaseTests
    {
        [Fact]
        public async Task Get_WhenMovieExists_ReturnMovie()
        {
            var service = new MovieService(MockUnitOfWork.Object, MockMovieBlobStorageRepository.Object);

            var result = await service.Get(Movie.Id);

            result.Should().BeOfType<MovieDTO>();
            MockMoviesRepository.Verify(x => x.GetAsync(Movie.Id));
        }

        [Fact]
        public async Task Get_WhenMovieDoesNotExist_ReturnMovie()
        {
            var service = new MovieService(MockUnitOfWork.Object, MockMovieBlobStorageRepository.Object);

            var act = async () => await service.Get(Guid.NewGuid());

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAll_WhenMoviesExist_ReturnMovies()
        {
            MoviesList.Add(Movie);

            var service = new MovieService(MockUnitOfWork.Object, MockMovieBlobStorageRepository.Object);
            var result = await service.GetAll();

            result.Should().BeOfType<List<MovieDTO>>();
            result.Should().HaveCount(1);
            MockMoviesRepository.Verify(x => x.GetAllAsync());
        }

        [Fact]
        public async Task GetAll_WhenMoviesDoNotExist_ReturnEmptyList()
        {
            var service = new MovieService(MockUnitOfWork.Object, MockMovieBlobStorageRepository.Object);
            var result = await service.GetAll();

            result.Should().BeEmpty();
            MockMoviesRepository.Verify(x => x.GetAllAsync());
        }
    }
}
