using IA.DevOps.Movies.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IA.DevOps.Movies.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IHealthCheckService, HealthCheckService>();

            return services;
        }
    }
}
