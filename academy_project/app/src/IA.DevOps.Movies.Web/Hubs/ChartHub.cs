using IA.DevOps.Movies.Contracts.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace IA.DevOps.Movies.Web.Hubs
{
    public class ChartHub : Hub
    {
        public async Task SendMessage(SystemInformationDTO messageData)
        {
            await Clients.All.SendAsync("UpdateSystemInformation", messageData);
        }
    }
}
