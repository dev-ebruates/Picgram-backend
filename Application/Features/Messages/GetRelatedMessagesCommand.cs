
namespace Application.Features.Messages;

public class GetRelatedMessagesCommand : IRequest<List<GetRelatedMessagesCommandResponse>>
{
  public Guid ReceiverUserId { get; set; }

  public class GetRelatedMessagesCommandHandler : IRequestHandler<GetRelatedMessagesCommand, List<GetRelatedMessagesCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;

    public GetRelatedMessagesCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
    }

    public async Task<List<GetRelatedMessagesCommandResponse>> Handle(GetRelatedMessagesCommand request, CancellationToken cancellationToken)
    {
      var senderUserId = httpUserService.GetUserId();
      var receiverUserId = request.ReceiverUserId;
      var messages = await unitOfWork.MessageRepository.GetRelatedMessages(senderUserId, receiverUserId);
      var response = messages.Select(m => new GetRelatedMessagesCommandResponse(
        m.Sender.Username,
        m.Sender.ProfilePicture,
        m.Receiver.Username,
        m.Receiver.ProfilePicture,
        m.Content,
        m.CreatedAt)).ToList();
      return response;
    }
  }
}


public class GetRelatedMessagesCommandResponse
{
  public GetRelatedMessagesCommandResponse(string senderUsername, string? senderUserProfilePicture, string receiverUsername, string? receiverUserProfilePicture, string content, DateTime createdAt)
  {
    SenderUsername = senderUsername;
    SenderUserProfilePicture = senderUserProfilePicture;
    ReceiverUsername = receiverUsername;
    ReceiverUserProfilePicture = receiverUserProfilePicture;
    Content = content;
    CreatedAt = createdAt;
  }

  public string SenderUsername { get; set; }
  public string? SenderUserProfilePicture { get; set; }
  public string ReceiverUsername { get; set; }
  public string? ReceiverUserProfilePicture { get; set; }
  public string Content { get; set; }
  public DateTime CreatedAt { get; set; }
}