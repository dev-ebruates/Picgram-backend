
namespace Application.Features.Search;

public class GetSearchCommand : IRequest<Response<List<GetSearchCommandResponse>>>
{
  public string? searchParameter { get; set; }
  public class GetSearchCommandHandler : IRequestHandler<GetSearchCommand, Response<List<GetSearchCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;
    public GetSearchCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }
    public async Task<Response<List<GetSearchCommandResponse>>> Handle(GetSearchCommand request, CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(request.searchParameter))
        throw new ArgumentNullException(nameof(request.searchParameter));
      var users = await unitOfWork.UserRepository.GetAllSearch(request.searchParameter);
      if (users == null)
        return Response<List<GetSearchCommandResponse>>.CreateSuccessResponse(new List<GetSearchCommandResponse>(0), "No users found");
      return Response<List<GetSearchCommandResponse>>.CreateSuccessResponse(users.Select(u => new GetSearchCommandResponse(u.Id, u.Username, u.ProfilePicture)).ToList());
    }
  }
}

public class GetSearchCommandResponse
{
  public Guid Id { get; set; }
  public string? Username { get; set; }
 public string? ProfilePicture { get; set; }
  public GetSearchCommandResponse(Guid id, string? username, string? profilePicture)
  {
    Id = id;
    Username = username;
    ProfilePicture = profilePicture;
  }

}