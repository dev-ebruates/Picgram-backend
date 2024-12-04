using Application.Commons;

namespace Application.Features.Users;

public class CreateUserCommand : IRequest<Response<CreateUserCommandResponse>>
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool Validation()
    {
        return Password != null && Password.Length > 3 && Username != null && Username.Length > 1 && Email != null && Email.Length > 0 && Email.Contains("@");
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<CreateUserCommandResponse>>
    {
        private readonly UnitOfWork unitOfWork;

        public CreateUserCommandHandler(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Response<CreateUserCommandResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.Validation())
                    throw new ArgumentException("Invalid request");
                var user = User.Create(request.Username, request.Email, request.Password);
                var dbUser = unitOfWork.UserRepository.Add(user);
                await unitOfWork.SaveChanges(cancellationToken);
                var response = new CreateUserCommandResponse(dbUser.Username);
                return Response<CreateUserCommandResponse>.CreateSuccessResponse(response, "User created successfully");
            }
            catch (Exception)
            {
                return Response<CreateUserCommandResponse>.CreateErrorResponse("Is username or email already in use?");
            }
        }
    }
}

public class CreateUserCommandResponse
{
    public CreateUserCommandResponse(string username)
    {
        Username = username;
    }

    public string Username { get; set; } = null!;
}

