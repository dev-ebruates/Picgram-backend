using System;

namespace Application.Features.Users;




public class UpdateUserProfilePictureCommand : IRequest<Response<UpdateUserProfilePictureCommandResponse>>
{
  public string? ProfilePicture { get; set; }

  public class UpdateUserProfilePictureCommandHandler : IRequestHandler<UpdateUserProfilePictureCommand, Response<UpdateUserProfilePictureCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UpdateUserProfilePictureCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<UpdateUserProfilePictureCommandResponse>> Handle(UpdateUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new Exception("User not found");
        var user = await unitOfWork.UserRepository.Get(Guid.Parse(userId))
          ?? throw new Exception("User not found");
        user.UpdateProfilePicture(request.ProfilePicture);
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<UpdateUserProfilePictureCommandResponse>.CreateSuccessResponse(new UpdateUserProfilePictureCommandResponse(user.ProfilePicture), "User ProfilePicture updated successfully");

      }
      catch (Exception)
      {
        return Response<UpdateUserProfilePictureCommandResponse>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class UpdateUserProfilePictureCommandResponse
{
  public UpdateUserProfilePictureCommandResponse(string? profilePicture)
  {
    ProfilePicture = profilePicture;
  }

  public string? ProfilePicture { get; set; } = null!;
}


