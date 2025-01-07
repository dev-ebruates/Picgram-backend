namespace Application.Features.Users;

public class GetMyProfileCommand : IRequest<Response<GetMyProfileCommandResponse>>
{

  public class GetMyProfileCommandHandler : IRequestHandler<GetMyProfileCommand, Response<GetMyProfileCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;
    public GetMyProfileCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<GetMyProfileCommandResponse>> Handle(GetMyProfileCommand request, CancellationToken cancellationToken)
    {
      var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
   ?? throw new Exception("User not found");
      var user = await unitOfWork.UserRepository.Get(Guid.Parse(userId))
        ?? throw new Exception("User not found");
      await unitOfWork.SaveChanges(cancellationToken);
      return Response<GetMyProfileCommandResponse>.CreateSuccessResponse(new GetMyProfileCommandResponse(user.Username, user.ProfilePicture, user.Bio, user.Role), "Get user my-profile successfully");
    }
  }
}

public class GetMyProfileCommandResponse
{
  public string Username { get; set; }
  public string? UserProfilePicture { get; set; }
  public string? Bio { get; set; } = "";
  public UserRole Role { get; set; } 
  public GetMyProfileCommandResponse(string username, string? userProfilePicture, string? bio, UserRole role)
  {
    Username = username;
    UserProfilePicture = userProfilePicture;
    Bio = bio;
    Role = role;
  }
}