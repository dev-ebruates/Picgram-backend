namespace Application.Features.Users;

public class GetProfileCommand : IRequest<Response<GetProfileCommandResponse>>
{
  public string username { get; set; } = null!;
  public class GetProfileCommandHandler : IRequestHandler<GetProfileCommand, Response<GetProfileCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    public GetProfileCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
  
    }

    public async Task<Response<GetProfileCommandResponse>> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
      try
      {
        
        var user = await unitOfWork.UserRepository.GetByUsername(request.username)
          ?? throw new Exception("User not found");
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<GetProfileCommandResponse>.CreateSuccessResponse(new GetProfileCommandResponse(user.Username, user.ProfilePicture, user.Bio), "My profile retrieved successfully");
      }
      catch (Exception)
      {
        return Response<GetProfileCommandResponse>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class GetProfileCommandResponse
{
  public string Username { get; set; }
  public string? UserProfilePicture { get; set; }
  public string? Bio { get; set; } = "";
  public GetProfileCommandResponse(string username, string? userProfilePicture, string? bio)
  {
    Username = username;
    UserProfilePicture = userProfilePicture;
    Bio = bio;
  }
}