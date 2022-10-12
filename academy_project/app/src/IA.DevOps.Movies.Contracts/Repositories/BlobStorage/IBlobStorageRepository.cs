namespace IA.DevOps.Movies.Contracts.Repositories.BlobStorage
{
    public interface IBlobStorageRepository
    {
        Task Add(string blobName, Stream content);
        string GetFileUrl(string blobName);
        Task Delete(string blobName);
    }
}
