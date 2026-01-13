using Microsoft.AspNetCore.SignalR;

namespace Pokeguesser.Hubs
{
    public class ConnectionHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}