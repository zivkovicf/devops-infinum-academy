using IA.DevOps.Movies.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IA.DevOps.Movies.Data.Db
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMoviesDbServices(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<MoviesDbContext>(
                options =>
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        options.UseInMemoryDatabase("movies-db");
                    }
                    else
                    {
                        options.UseNpgsql(connectionString);
                    }
                });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
