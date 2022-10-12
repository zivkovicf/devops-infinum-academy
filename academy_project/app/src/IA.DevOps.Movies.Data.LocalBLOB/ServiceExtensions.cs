using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace IA.DevOps.Movies.Data.LocalBLOB
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLocalBlobStorageServices(this IServiceCollection services)
        {
            services.AddScoped<IMovieBlobStorageRepository, MoviesBlobStorageRepository>();

            return services;
        }
    }
}
