namespace Application.Services;

public class SignalRUserService
{
  private readonly IHubContext<NotificationHub> hubContext;
  private readonly List<UserClient> userClients;

  public SignalRUserService(IHubContext<NotificationHub> hubContext)
  {
    userClients = [];
    this.hubContext = hubContext;
  }

  public void AddClient(string username, string client)
  {
    var userClient = userClients.FirstOrDefault(x => x.Username == username);
    if (userClient == null)
    {
      userClients.Add(new UserClient { Username = username, Clients = new List<string> { client } });
    }
    else
    {
      userClient.Clients.Add(client);
    }
  }

  public void RemoveClient(string username, string clientId)
  {
    var userClient = userClients.FirstOrDefault(x => x.Username == username);
    if (userClient != null)
    {
      var client = userClient.Clients.FirstOrDefault(x => x == clientId);
      if (client != null)
        userClient.Clients.Remove(client);
    }
    if (userClient?.Clients?.Count <= 0)
      userClients.Remove(userClient);
  }

  public async Task SendNotification(string username, string methodName, string? payload = null)
  {
    var userClient = userClients?.FirstOrDefault(x => x.Username == username);
    if (userClient != null)
    {
      foreach (var client in userClient.Clients)
      {
        var clientProxy = hubContext?.Clients?.Client(client);
        if (clientProxy != null)
        {
          if (payload == null)
            await clientProxy.SendAsync("ReceiveNotification", methodName);
          else
            await clientProxy.SendAsync("ReceiveNotification", methodName, payload);
        }
      }
    }
  }
}


public class UserClient
{
  public string Username { get; set; }
  public List<string> Clients { get; set; }
}