using IA.DevOps.Movies.Contracts.Factories;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Data.AwsS3.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IA.DevOps.Movies.Data.AwsS3
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAwsS3StorageServices(this IServiceCollection services)
        {
            services.AddScoped<IMovieBlobStorageRepository, MoviesBlobStorageRepository>();
            services.AddScoped<IAwsS3BlobStorageFactory, AwsS3BlobStorageFactory>();

            return services;
        }
    }
}
