using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;
using IA.DevOps.Movies.Data.LocalBLOB.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace IA.DevOps.Movies.Data.LocalBLOB
{
    public abstract class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private string FolderPath => Path.Combine(_hostingEnvironment.WebRootPath, FolderNames.RootFolder, FolderName);

        public abstract string FolderName { get; }

        public BlobStorageRepository(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public Task Add(string blobName, Stream content)
        {
            CreateFolderIfNotExists();

            var filePath = Path.Combine(FolderPath, blobName);

            using var fileStream = File.Create(filePath, (int)content.Length);
            content.CopyTo(fileStream);

            return Task.CompletedTask;
        }

        public Task Delete(string blobName)
        {
            File.Delete(Path.Combine(FolderPath, blobName));

            return Task.CompletedTask;
        }

        public string GetFileUrl(string blobName)
        {
            return Path.Combine("/", FolderNames.RootFolder, FolderName, blobName);
        }

        private void CreateFolderIfNotExists()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }
    }
}
