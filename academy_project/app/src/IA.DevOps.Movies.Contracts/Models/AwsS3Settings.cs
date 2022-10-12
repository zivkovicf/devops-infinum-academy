namespace IA.DevOps.Movies.Contracts.Models
{
    public class AwsS3Settings
    {
        public string AccessKeyId { get; set; } = default!;
        public string SecretAccessKey { get; set; } = default!;
        public string Region { get; set; } = default!;
        public string ServiceURL { get; set; } = default!;
    }
}
