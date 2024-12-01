namespace Application.Features.Users;

public class CreateUserCommand : IRequest<CreateUserCommandResponse>
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool Validation()
    {
        return Password != null && Password.Length > 3 && Username != null && Username.Length > 1 && Email != null && Email.Length > 0 && Email.Contains("@");
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
    {
        private readonly UnitOfWork unitOfWork;

        public CreateUserCommandHandler(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.Validation())
                throw new ArgumentException("Invalid request");
            var user = User.Create(request.Username, request.Email, request.Password);
            var dbUser = unitOfWork.UserRepository.Add(user);
            unitOfWork.SaveChanges(cancellationToken);
            var response = new CreateUserCommandResponse(dbUser.Username, "User created successfully");
            return Task.FromResult(response);
        }
    }
}

public class CreateUserCommandResponse
{
    public CreateUserCommandResponse(string username, string message)
    {
        Username = username;
        Message = message;
    }

    public string Username { get; set;} = null!;
    public string Message { get; set; } = null!;
}

