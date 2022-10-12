using IA.DevOps.Movies.Contracts.Services;
using IA.DevOps.Movies.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace IA.DevOps.Movies.Web
{
    public class HealthMonitor : BackgroundService
    {
        private readonly IHubContext<ChartHub> _hub;
        private readonly IHealthCheckService _healthCheckService;

        public HealthMonitor(IHubContext<ChartHub> hub, IServiceScopeFactory scopeFactory)
        {
            _hub = hub;
            _healthCheckService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHealthCheckService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var stats = await _healthCheckService.GetCurrentStats();

                await _hub.Clients.All.SendAsync("UpdateSystemInformation", stats);
            }
        }
    }
}
