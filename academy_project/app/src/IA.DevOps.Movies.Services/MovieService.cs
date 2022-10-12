using IA.DevOps.Movies.Common.Exceptions.Types;
using IA.DevOps.Movies.Contracts;
using IA.DevOps.Movies.Contracts.DTOs;
using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Contracts.Forms;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Contracts.Services;

namespace IA.DevOps.Movies.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMovieBlobStorageRepository _movieBlobStorageRepository;

        public MovieService(IUnitOfWork unitOfWork, IMovieBlobStorageRepository movieBlobStorageRepository)
        {
            _unitOfWork = unitOfWork;
            _movieBlobStorageRepository = movieBlobStorageRepository;
        }

        public async Task<MovieDTO> Get(Guid id)
        {
            var movie = await _unitOfWork.Movies.GetAsync(id);

            if (movie == null)
            {
                throw new NotFoundException();
            }

            return MapMovieToDTO(movie);
        }

        public async Task<List<MovieDTO>> GetAll()
        {
            var movies = await _unitOfWork.Movies.GetAllAsync();

            return movies.OrderByDescending(x => x.Rating)
                         .Select(movie => MapMovieToDTO(movie))
                         .ToList();
        }

        public async Task<MovieDTO> Create(MovieForm movieForm)
        {
            var movie = new Movie
            {
                Title = movieForm.Title,
                Genre = movieForm.Genre,
                ReleasedYear = movieForm.ReleasedYear,
                Rating = movieForm.Rating,
                Overview = movieForm.Overview
            };

            _unitOfWork.Movies.Add(movie);
            await _unitOfWork.Movies.SaveChangesAsync();

            if (movieForm.Image != null)
            {
                await _movieBlobStorageRepository.Add(movie.Id.ToString(), movieForm.Image);
            }

            return MovieDTO.FromMovie(movie);
        }

        public async Task<MovieDTO> Update(MovieForm movieForm)
        {
            if (movieForm.Id == null)
            {
                throw new NotFoundException();
            }

            var movie = await _unitOfWork.Movies.GetAsync(movieForm.Id.Value);

            if (movie == null)
            {
                throw new NotFoundException();
            }

            movie.Title = movieForm.Title;
            movie.Genre = movieForm.Genre;
            movie.ReleasedYear = movieForm.ReleasedYear;
            movie.Rating = movieForm.Rating;
            movie.Overview = movieForm.Overview;

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync();

            if (movieForm.Image != null)
            {
                await _movieBlobStorageRepository.Add(movie.Id.ToString(), movieForm.Image);
            }

            return MovieDTO.FromMovie(movie);
        }

        private MovieDTO MapMovieToDTO(Movie movie)
        {
            var movieDto = MovieDTO.FromMovie(movie);
            movieDto.ImageUrl = _movieBlobStorageRepository.GetFileUrl(movie.Id.ToString());

            return movieDto;
        }
    }
}
