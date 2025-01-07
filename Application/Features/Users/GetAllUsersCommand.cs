namespace Application.Features.Users;

public class GetAllUsersCommand : IRequest<Response<List<GetAllUsersCommandResponse>>>
{

  public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, Response<List<GetAllUsersCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;
    public GetAllUsersCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetAllUsersCommandResponse>>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var users = await this.unitOfWork.UserRepository.GetAll();
        if(users == null)
          return Response<List<GetAllUsersCommandResponse>>.CreateSuccessResponse(new List<GetAllUsersCommandResponse>(0), "No users found");

          var userResponse = users.Select(u => new GetAllUsersCommandResponse(u.Id, u.Username, u.ProfilePicture, u.Role, u.CreatedAt)).ToList().OrderByDescending(x => x.CreatedAt)
          .ToList();
        return Response<List<GetAllUsersCommandResponse>>.CreateSuccessResponse(userResponse);
      }
      catch (Exception)
      {
        return Response<List<GetAllUsersCommandResponse>>.CreateErrorResponse("Invalid request");
      }

    }
  }
}

public class GetAllUsersCommandResponse
{
  public Guid Id { get; set; }
  public string Username { get; set; }
  public string? UserProfilePicture { get; set; }
  public UserRole Role { get; set; }
  public DateTime CreatedAt { get; set; }


  public GetAllUsersCommandResponse(Guid id, string username, string? userProfilePicture, UserRole role, DateTime createdAt)
  {
  Id = id;
  Username = username;
  UserProfilePicture = userProfilePicture;
  Role = role;
  CreatedAt = createdAt;
  }
}