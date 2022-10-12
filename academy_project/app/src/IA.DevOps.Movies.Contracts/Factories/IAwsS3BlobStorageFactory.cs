using Amazon.S3;

namespace IA.DevOps.Movies.Contracts.Factories
{
    public interface IAwsS3BlobStorageFactory
    {
        AmazonS3Client CreateBlobServiceClient();
    }
}
