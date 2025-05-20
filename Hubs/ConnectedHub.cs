using Microsoft.AspNetCore.SignalR;

namespace MyPhamSoul.Hubs
{
    public class ConnectedHub : Hub
    {


        public async Task SendMessage(string user, string message)
        {

            await Clients.All.SendAsync("ReceiveMessage", user, message);

        }

    }
}