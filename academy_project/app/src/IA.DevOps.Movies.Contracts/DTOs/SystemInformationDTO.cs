using IA.DevOps.Movies.Contracts.Models;

namespace IA.DevOps.Movies.Contracts.DTOs
{
    public class SystemInformationDTO
    {
        public double PhysicalMemoryUsage { get; set; }
        public double TotalMemorySize { get; set; }
        public TimeSpan TotalProcessorTime { get; set; }
        public TimeSpan UserProcessorTime { get; set; }
        public TimeSpan PrivilegedProcessorTime { get; set; }
        public double CPUUtilization { get; set; }
        public bool DatabaseOnline { get; set; }
        public AwsS3Settings AWSS3Settings { get; set; } = default!;
        public string[] BucketsList { get; set; } = default!;
        public string DatabaseHost { get; set; } = default!;
        public string AwsS3ServiceUrl { get; set; } = default!;
        public KeyValuePair<string, string>[] CustomEnvVariables { get; set; } = default!;
    }
}
