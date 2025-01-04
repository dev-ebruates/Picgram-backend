
namespace Application.Features.Messages;

public class CreateMessageCommand : IRequest<Response<CreateMessageCommandResponse>>
{
  public Guid ReceiverUserId { get; set; }
  public string Content { get; set; }

  public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Response<CreateMessageCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;

    public CreateMessageCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
    }

    public async Task<Response<CreateMessageCommandResponse>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
      var senderUserId = httpUserService.GetUserId();
      var receiverUserId = request.ReceiverUserId;
      var message = Message.Create(senderUserId, receiverUserId, request.Content);
      unitOfWork.MessageRepository.Add(message);
      await unitOfWork.SaveChanges(cancellationToken);
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