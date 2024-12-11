namespace Application.Features.Users;

public class UpdateUserBioCommand : IRequest<Response<UpdateUserBioCommandResponse>>
{
  public string? Bio { get; set; }

  public class UpdateUserBioCommandHandler : IRequestHandler<UpdateUserBioCommand, Response<UpdateUserBioCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UpdateUserBioCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<UpdateUserBioCommandResponse>> Handle(UpdateUserBioCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new Exception("User not found");
        var user = await unitOfWork.UserRepository.Get(Guid.Parse(userId))
          ?? throw new Exception("User not found");
        user.UpdateBio(request.Bio);
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<UpdateUserBioCommandResponse>.CreateSuccessResponse(new UpdateUserBioCommandResponse(user.Bio), "User bio updated successfully");

      }
      catch (Exception)
      {
        return Response<UpdateUserBioCommandResponse>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class UpdateUserBioCommandResponse
{
  public UpdateUserBioCommandResponse(string? bio)
  {
    Bio = bio;
  }

  public string? Bio { get; set; } = null!;
}

