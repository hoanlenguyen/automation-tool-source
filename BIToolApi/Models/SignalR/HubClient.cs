using BITool.Enums;
using Microsoft.AspNetCore.SignalR;

namespace BITool.Models.SignalR
{
    public interface IHubClient
    {
    }

    public class HubClient : Hub, IHubClient
    {
        public HubClient()
        {
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"ConnectionId: {connectionId}");
            if (Clients != null)
            {
                await Clients.Client(connectionId).SendAsync(HubClientName.GetConnectionId, connectionId);
            }
            else
            {
                Console.WriteLine($"Clients has no data");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception = null)
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"ConnectionId OnDisconnectedAsync: {connectionId}");
            //if (Clients != null)
            //{
            //    await Clients.Client(connectionId).SendAsync("getConnectionId", connectionId);
            //}
            //else
            //{
            //    Console.WriteLine($"Clients has no data");
            //}
            await base.OnDisconnectedAsync(exception);

        }
        public async Task  GetCurrentConnectionId()
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"ConnectionId: {connectionId}");
            if (Clients != null)
            {
                await Clients.Client(connectionId).SendAsync(HubClientName.GetConnectionId, connectionId);
            }
            else
            {
                Console.WriteLine($"Clients has no data");
            }
        }
    }
}