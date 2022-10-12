using IA.DevOps.Movies.Contracts.DTOs;
using IA.DevOps.Movies.Contracts.Models;
using IA.DevOps.Movies.Contracts.Services;
using IA.DevOps.Movies.Data.Db;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Diagnostics;

namespace IA.DevOps.Movies.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly AwsS3Settings _awsS3Settings;
        private readonly DatabaseConnectionSettings _dbSettings;
        private readonly MoviesDbContext _dbContext;

        public HealthCheckService(
            MoviesDbContext context,
            IOptions<AwsS3Settings> awsOptions,
            IOptions<DatabaseConnectionSettings> dbOptions)
        {
            _dbContext = context;
            _awsS3Settings = awsOptions.Value;
            _dbSettings = dbOptions.Value;
        }

        public async Task<SystemInformationDTO> GetCurrentStats()
        {
            return new SystemInformationDTO
            {
                PhysicalMemoryUsage = Process.WorkingSet64,
                PrivilegedProcessorTime = Process.PrivilegedProcessorTime,
                UserProcessorTime = Process.UserProcessorTime,
                TotalProcessorTime = Process.TotalProcessorTime,
                CPUUtilization = await CalculateCPUUtilization(),
                TotalMemorySize = TotalMemorySize,
                DatabaseOnline = _dbContext.Database.CanConnect(),
                DatabaseHost = ReadDatabaseHostname(),
                AwsS3ServiceUrl = AwsS3ServiceUrl(),
                CustomEnvVariables = FetchCustomEnvVariables()
            };
        }

        private static Process Process => Process.GetCurrentProcess();

        private static KeyValuePair<string, string>[] FetchCustomEnvVariables()
        {
            var customVariables = new List<KeyValuePair<string, string>>();

            foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
            {
                var envVariable = new KeyValuePair<string, string>(e.Key.ToString(), e.Value.ToString());

                if (envVariable.Key.ToUpper().StartsWith("ACADEMY"))
                {
                    customVariables.Add(envVariable);
                }
            }

            return customVariables.ToArray();
        }

        private string ReadDatabaseHostname()
        {
            if (string.IsNullOrEmpty(_dbSettings.MoviesDB))
            {
                return "In-Memory";
            }

            return _dbSettings.MoviesDB
                .Split(";")
                .FirstOrDefault(x => x.StartsWith("Host") || x.StartsWith("Server"), "Server=Unknown")
                .Split("=")[1];
        }

        private string AwsS3ServiceUrl()
        {
            if (string.IsNullOrEmpty(_awsS3Settings.ServiceURL))
            {
                return "Local disk storage";
            }

            return _awsS3Settings.ServiceURL;
        }

        private static async Task<double> CalculateCPUUtilization()
        {
            var process = Process.GetCurrentProcess();

            var startTime = DateTime.UtcNow;
            var startCpuUsage = process.TotalProcessorTime;

            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = process.TotalProcessorTime;

            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;

            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            return cpuUsageTotal * 100;
        }

        private static double TotalMemorySize => GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
    }
}
