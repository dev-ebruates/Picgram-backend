
namespace Application.Features.Auth;

public class AuthCommand : IRequest<Response<AuthCommandResponse>>
{
  public string EmailOrUsername { get; set; } = null!;
  public string Password { get; set; } = null!;

  public class AuthCommandHandler : IRequestHandler<AuthCommand, Response<AuthCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly JwtTokenService jwtTokenService;

    public AuthCommandHandler(UnitOfWork unitOfWork, JwtTokenService jwtTokenService)
    {
      this.unitOfWork = unitOfWork;
      this.jwtTokenService = jwtTokenService;
    }

    public async Task<Response<AuthCommandResponse>> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
      var user = await unitOfWork.UserRepository.GetByEmailOrUsername(request.EmailOrUsername);
      if (user == null)
        return Response<AuthCommandResponse>.CreateErrorResponse("User not found");
      if (user.Password != request.Password)
        return Response<AuthCommandResponse>.CreateErrorResponse("Invalid password");
      var token = jwtTokenService.GenerateToken(user);
      return Response<AuthCommandResponse>.CreateSuccessResponse(new AuthCommandResponse(token, user.Username, user.Role), "User authenticated successfully");
    }
  }
}

public class AuthCommandResponse
{
  public AuthCommandResponse(string token, string userName,  UserRole role)
  {
    Token = token;
    Username = userName;
    Role = role;

  }

  public string Token { get; set; }
  public string Username { get; set; }
  public UserRole Role { get; set; }

}
