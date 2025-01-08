namespace Application.Features.Users;

public class DeleteUserCommand : IRequest<Response<DeleteUserCommandResponse>>
{
  public Guid UserId { get; set; }
  public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<DeleteUserCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;

    public DeleteUserCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<DeleteUserCommandResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      try
      {
        // Kullanıcıyı silmek için repository'yi çağır
        await unitOfWork.UserRepository.SoftDelete(request.UserId);
        await unitOfWork.SaveChanges(cancellationToken);
        // Yanıt döndür
        return Response<DeleteUserCommandResponse>.CreateSuccessResponse();
      }
      catch (Exception ex)
      {
        return Response<DeleteUserCommandResponse>.CreateErrorResponse("Invalid request");
      }

    }
  }
}
public class DeleteUserCommandResponse
{
}
