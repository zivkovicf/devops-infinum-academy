using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Data.LocalBLOB.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace IA.DevOps.Movies.Data.LocalBLOB
{
    public class MoviesBlobStorageRepository : BlobStorageRepository, IMovieBlobStorageRepository
    {
        public override string FolderName => FolderNames.Movies;

        public MoviesBlobStorageRepository(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment)
        {
        }
    }
}
