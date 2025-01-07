namespace API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly SignalRUserService signalRUserService;

    public NotificationHub(SignalRUserService signalRUserService)
    {
        this.signalRUserService = signalRUserService;
    }

    public override Task OnConnectedAsync()
    {
        try
        {
            if (Context.User.Identity!.IsAuthenticated)
                signalRUserService.AddClient(Context.User.Identity!.Name!, Context.ConnectionId);
            else
            {
                Context?.Abort();
            }
        }
        catch (Exception)
        {
            Context?.Abort();
        }

        return base.OnConnectedAsync();
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            if (Context.User.Identity!.IsAuthenticated)
                signalRUserService.RemoveClient(Context.User.Identity!.Name!, Context.ConnectionId);
        }
        catch (Exception)
        {

        }
        return base.OnDisconnectedAsync(exception);
    }
}