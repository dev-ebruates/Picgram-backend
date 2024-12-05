
namespace Application.Features.Auth;

public class AuthCommand : IRequest<Response<AuthCommandResponse>>
{
  public string EmailOrUsername { get; set; } = null!;
  public string Password { get; set; } = null!;

  public class AuthCommandHandler : IRequestHandler<AuthCommand, Response<AuthCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;

    public AuthCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<AuthCommandResponse>> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
      var user = await unitOfWork.UserRepository.GetByEmailOrUsername(request.EmailOrUsername);
      if (user == null)
        return Response<AuthCommandResponse>.CreateErrorResponse("User not found");
      if (user.Password != request.Password)
        return Response<AuthCommandResponse>.CreateErrorResponse("Invalid password");
      return Response<AuthCommandResponse>.CreateSuccessResponse(new AuthCommandResponse("tokın"), "User authenticated successfully");
    }
  }
}

public class AuthCommandResponse
{
  public AuthCommandResponse(string token)
  {
    Token = token;
  }

  public string Token { get; set; }
}
