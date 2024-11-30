namespace Application.Features.Users;

public class CreateUserCommand : IRequest<CreateUserCommandResponse>
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
    {
        public Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

public class CreateUserCommandResponse
{

}

