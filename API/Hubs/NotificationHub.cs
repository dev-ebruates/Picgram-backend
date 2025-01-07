namespace API.Hubs;

public class NotificationHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task SendNotification(string notificationType)
    {
        await Clients.All.SendAsync("ReceiveNotification", notificationType);
    }
}