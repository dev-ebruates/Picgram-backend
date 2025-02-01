
namespace Application.Features.Messages;

public class CreateMessageCommand : IRequest<Response<CreateMessageCommandResponse>>
{
  public Guid ReceiverUserId { get; set; }
  public string Content { get; set; }

  public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Response<CreateMessageCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;
    private readonly SignalRUserService signalRUserService;

    public CreateMessageCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService, SignalRUserService signalRUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
      this.signalRUserService = signalRUserService;
    }

    public async Task<Response<CreateMessageCommandResponse>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
      var senderUserId = httpUserService.GetUserId();
      var receiverUserId = request.ReceiverUserId;
      var receiverUser = await unitOfWork.UserRepository.Get(receiverUserId) ?? throw new Exception("User not found");
      var senderUser = await unitOfWork.UserRepository.Get(senderUserId) ?? throw new Exception("User not found");

      var message = Message.Create(senderUserId, receiverUserId, request.Content);
      unitOfWork.MessageRepository.Add(message);
      await unitOfWork.SaveChanges(cancellationToken);
      
      await signalRUserService.SendNotification(receiverUser.Username, "CreateMessage");
      await signalRUserService.SendNotification(senderUser.Username, "CreateMessage");
      await signalRUserService.SendNotification(receiverUser.Username, "NewMessage", receiverUser.Username);


      return Response<CreateMessageCommandResponse>.CreateSuccessResponse(
        new CreateMessageCommandResponse(
          receiverUserId,
          senderUserId,
          request.Content),
        "Message sent successfully");
    }
  }
}

public class CreateMessageCommandResponse
{
  public CreateMessageCommandResponse(Guid receiverUserId, Guid senderUserId, string content)
  {
    ReceiverUserId = receiverUserId;
    SenderUserId = senderUserId;
    Content = content;
  }

  public Guid ReceiverUserId { get; }
  public Guid SenderUserId { get; }
  public string Content { get; }
}