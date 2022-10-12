using IA.DevOps.Movies.Contracts.Entities;

namespace IA.DevOps.Movies.Contracts.DTOs
{
    public class MovieDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;
        public int ReleasedYear { get; set; }
        public string Genre { get; set; } = default!;
        public string Overview { get; set; } = default!;
        public double Rating { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string ImageUrl { get; set; } = default!;

        public static MovieDTO FromMovie(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                ReleasedYear = movie.ReleasedYear,
                Genre = movie.Genre,
                Overview = movie.Overview,
                Rating = movie.Rating,
                CreatedAt = movie.CreatedAt,
                UpdatedAt = movie.UpdatedAt
            };
        }

        public static IEnumerable<MovieDTO> FromMovie(IEnumerable<Movie> movies)
        {
            return movies.Select(m => FromMovie(m));
        }
    }
}
