using Amazon.S3;
using IA.DevOps.Movies.Contracts.Factories;
using IA.DevOps.Movies.Contracts.Models;
using Microsoft.Extensions.Options;

namespace IA.DevOps.Movies.Data.AwsS3.Configuration
{
    internal class AwsS3BlobStorageFactory : IAwsS3BlobStorageFactory
    {
        private readonly AwsS3Settings _awsS3Settings;

        public AwsS3BlobStorageFactory(IOptions<AwsS3Settings> options)
        {
            _awsS3Settings = options.Value;
        }

        public AmazonS3Client CreateBlobServiceClient()
        {
            var awsS3Config = new AmazonS3Config
            {
                ServiceURL = _awsS3Settings.ServiceURL,
                ForcePathStyle = true
            };

            return new AmazonS3Client(
                awsAccessKeyId: _awsS3Settings.AccessKeyId,
                awsSecretAccessKey: _awsS3Settings.SecretAccessKey,
                clientConfig: awsS3Config);
        }
    }
}
