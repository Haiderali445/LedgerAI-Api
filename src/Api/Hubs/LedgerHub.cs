using Microsoft.AspNetCore.SignalR;

namespace LedgerAI.Api.Hubs;

public class LedgerHub : Hub
{
    public async Task SendNotification(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }
}
