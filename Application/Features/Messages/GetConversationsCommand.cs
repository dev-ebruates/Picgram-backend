
namespace Application.Features.Messages;

public class GetConversationsCommand : IRequest<Response<List<GetConversationsCommandResponse>>>
{
  public class GetConversationsCommandHandler : IRequestHandler<GetConversationsCommand, Response<List<GetConversationsCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;

    public GetConversationsCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
    }

    public async Task<Response<List<GetConversationsCommandResponse>>> Handle(GetConversationsCommand request, CancellationToken cancellationToken)
    {
      var userId = httpUserService.GetUserId();
      var userIds = await unitOfWork.MessageRepository.GetConversations(userId);
      var users = await unitOfWork.UserRepository.GetAllByIds(userIds, cancellationToken);
      var response = users.Select(u => 
        new GetConversationsCommandResponse(u.Id, u.Username, u.ProfilePicture)).ToList();
      return Response<List<GetConversationsCommandResponse>>.CreateSuccessResponse(response, "Conversations retrieved successfully");
    }
  }
}

public class GetConversationsCommandResponse
{
  public Guid UserId { get; set; }
  public string Username { get; set; }
  public string? ProfilePicture { get; set; }

  public GetConversationsCommandResponse(Guid userId, string username, string? profilePicture)
  {
    UserId = userId;
    Username = username;
    ProfilePicture = profilePicture;
  }
}