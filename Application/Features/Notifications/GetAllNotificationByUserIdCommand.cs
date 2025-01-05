namespace Application.Features.Notifications;

public class GetAllNotificationByUserIdCommand : IRequest<Response<List<GetAllNotificationByUserIdResponse>>>
{
  public class GetAllNotificationByUserIdCommandHandler : IRequestHandler<GetAllNotificationByUserIdCommand, Response<List<GetAllNotificationByUserIdResponse>>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;

    public GetAllNotificationByUserIdCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
    }
    public async Task<Response<List<GetAllNotificationByUserIdResponse>>> Handle(GetAllNotificationByUserIdCommand request, CancellationToken cancellationToken)
    {
      var userId = httpUserService.GetUserId();
      var notifications = await unitOfWork.NotificationRepository.GetAllByUserId(userId);
      var response = notifications.Select(n => new GetAllNotificationByUserIdResponse(n.Id, n.TriggerUser.Username, n.Post?.MediaUrl, n.CreatedAt, n.Type)).ToList();
      return Response<List<GetAllNotificationByUserIdResponse>>.CreateSuccessResponse(response);
    }
  }
}

public class GetAllNotificationByUserIdResponse
{
  public GetAllNotificationByUserIdResponse(Guid id, string triggerUsername, string? mediaUrl, DateTime createdAt, NotificationType type)
  {
    Id = id;
    TriggerUsername = triggerUsername;
    MediaUrl = mediaUrl;
    CreatedAt = createdAt;
    Type = type;
  }

  public Guid Id { get; set; }
  public string TriggerUsername { get; private set; } = null!;
  public DateTime CreatedAt { get; set; }
  public NotificationType Type { get; set; }
  public string? MediaUrl { get; set; } = null!;
}