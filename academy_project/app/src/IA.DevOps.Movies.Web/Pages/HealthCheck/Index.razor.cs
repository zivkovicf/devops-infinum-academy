using IA.DevOps.Movies.Contracts.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IA.DevOps.Movies.Web.Pages.HealthCheck
{
    public partial class Index
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private HubConnection HubConnection { get; set; } = default!;
        private SystemInformationDTO SystemInformation { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await StartHubConnection();
            AddTransferChartDataListener();
        }

        private async Task StartHubConnection()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("chart-hub"))
                .Build();

            await HubConnection.StartAsync();
        }
        private void AddTransferChartDataListener()
        {
            HubConnection.On<SystemInformationDTO>("UpdateSystemInformation", (data) =>
            {
                SystemInformation = data;
                InvokeAsync(StateHasChanged);
            });
        }
    }
}
