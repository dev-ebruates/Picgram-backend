namespace Application.Features.Auth;

public class GoogleAuthCommand : IRequest<Response<GoogleAuthCommandResponse>>
{
    public string clientId { get; set; }
    public string credential { get; set; }
    public string select_by { get; set; }

    public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, Response<GoogleAuthCommandResponse>>
    {
        private readonly UnitOfWork unitOfWork;
        private readonly GoogleAuthService googleAuthService;
        private readonly JwtTokenService jwtTokenService;

        public GoogleAuthCommandHandler(UnitOfWork unitOfWork, GoogleAuthService googleAuthService, JwtTokenService jwtTokenService)
        {
            this.unitOfWork = unitOfWork;
            this.googleAuthService = googleAuthService;
            this.jwtTokenService = jwtTokenService;
        }

        public async Task<Response<GoogleAuthCommandResponse>> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
        {
            var claimsPrincipal = await googleAuthService.ValidateGoogleToken(request.credential);
            if (claimsPrincipal == null)
                return Response<GoogleAuthCommandResponse>.CreateErrorResponse("Invalid token");
            if(claimsPrincipal.FindFirstValue("email_verified") != "true")
                return Response<GoogleAuthCommandResponse>.CreateErrorResponse("Invalid token");
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Response<GoogleAuthCommandResponse>.CreateErrorResponse("Invalid token");

            var user = await unitOfWork.UserRepository.GetByEmailOrUsername(email);
            if (user == null)
            {
                var username = email.Split('@')[0];
                user = User.Create(username, email, Guid.NewGuid().ToString());
                user.UpdateProfilePicture(claimsPrincipal.FindFirstValue("picture"));
                unitOfWork.UserRepository.Add(user);
                await unitOfWork.SaveChanges(cancellationToken);
            }
            var token = jwtTokenService.GenerateToken(user);
            return Response<GoogleAuthCommandResponse>.CreateSuccessResponse(new GoogleAuthCommandResponse(token, user.Username, user.Role), "User authenticated successfully");
        }
    }
}

public class GoogleAuthCommandResponse
{
    public GoogleAuthCommandResponse(string token, string userName, UserRole role)
    {
        Token = token;
        Username = userName;
        Role = role;

    }

    public string Token { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
}
