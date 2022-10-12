using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using IA.DevOps.Movies.Contracts.Factories;
using IA.DevOps.Movies.Contracts.Repositories.BlobStorage;

namespace IA.DevOps.Movies.Data.AwsS3
{
    public abstract class BlobStorageRepository : IBlobStorageRepository
    {
        public abstract string BucketName { get; }
        public abstract double ExpiresMins { get; }

        private readonly AmazonS3Client _awsS3Client;

        public BlobStorageRepository(IAwsS3BlobStorageFactory blobStorageFactory)
        {
            _awsS3Client = blobStorageFactory.CreateBlobServiceClient();
        }

        public async Task Add(string blobName, Stream content)
        {
            await CreateBucketIfNotExists();

            var fileTransfer = new TransferUtility(_awsS3Client);

            await fileTransfer.UploadAsync(content, BucketName, blobName);
        }

        public async Task Delete(string blobName)
        {
            await _awsS3Client.DeleteObjectAsync(BucketName, blobName);
        }

        public string GetFileUrl(string blobName)
        {
            return _awsS3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = blobName,
                Expires = DateTime.UtcNow.AddMinutes(ExpiresMins)
            });
        }

        private async Task CreateBucketIfNotExists()
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(_awsS3Client, BucketName))
            {
                await _awsS3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = BucketName,
                    UseClientRegion = true
                });
            }
        }
    }
}
