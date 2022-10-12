using CsvHelper;
using CsvHelper.Configuration;
using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Data.Db.SeedData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace IA.DevOps.Movies.Data.Db.SeedData
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MoviesDbContext(serviceProvider.GetRequiredService<DbContextOptions<MoviesDbContext>>());
            if (!context.Database.CanConnect())
            {
                return;
            }

            context.Database.EnsureCreated();

            if (context.Movies.Any())
            {
                return;
            }

            var blobRepository = serviceProvider.GetRequiredService<IMovieBlobStorageRepository>();

            await ImportMoviesFromCSVAsync(context, blobRepository, resourceName: "IA.DevOps.Movies.Data.Db.SeedData.movies.csv");
        }

        private static async Task ImportMoviesFromCSVAsync(MoviesDbContext context, IMovieBlobStorageRepository blobRepository, string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                var csvReader = new CsvReader(reader, csvConfiguration);
                var csvMovies = csvReader.GetRecords<MovieCSV>();

                foreach (var csvMovie in csvMovies)
                {
                    var movie = new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = csvMovie.Title,
                        Genre = csvMovie.Genre,
                        Overview = csvMovie.Overview,
                        ReleasedYear = csvMovie.ReleasedYear,
                        Rating = csvMovie.IMDBRating
                    };

                    context.Movies.Add(movie);

                    using var client = new HttpClient();
                    var image = await client.GetAsync(csvMovie.PosterLink);
                    await blobRepository.Add(movie.Id.ToString(), image.Content.ReadAsStream());
                }
            }

            context.SaveChanges();
        }
    }
}
