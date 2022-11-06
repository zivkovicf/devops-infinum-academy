using IA.DevOps.Movies.Contracts.Factories;
using IA.DevOps.Movies.Contracts.Models;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using Microsoft.Extensions.Options;

namespace IA.DevOps.Movies.Data.AwsS3
{
    public class MoviesBlobStorageRepository : BlobStorageRepository, IMovieBlobStorageRepository
    {
        public override double ExpiresMins => 15;

        public MoviesBlobStorageRepository(
            IAwsS3BlobStorageFactory blobStorageFactory,
            IOptions<AwsS3Settings> options) : base(blobStorageFactory)
        {
            BucketName = options.Value.BucketName;
        }
    }
}
