using IA.DevOps.Movies.Contracts.Factories;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Data.AwsS3.Configuration;

namespace IA.DevOps.Movies.Data.AwsS3
{
    public class MoviesBlobStorageRepository : BlobStorageRepository, IMovieBlobStorageRepository
    {
        public override string BucketName => BucketNames.Movies;

        public override double ExpiresMins => 15;

        public MoviesBlobStorageRepository(IAwsS3BlobStorageFactory blobStorageFactory) : base(blobStorageFactory)
        {
        }
    }
}
