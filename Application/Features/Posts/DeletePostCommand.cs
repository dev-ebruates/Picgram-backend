namespace Application.Features.Posts;

public class DeletePostCommand : IRequest<Response<DeletePostCommandResponse>>
{
  public Guid PostId { get; set; }
  public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Response<DeletePostCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;

    public DeletePostCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<DeletePostCommandResponse>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var post = await unitOfWork.PostRepository.Get(request.PostId);
        if (post == null)
          return Response<DeletePostCommandResponse>.CreateErrorResponse("Post not found");
        post.Delete();
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<DeletePostCommandResponse>.CreateSuccessResponse();
      }
      catch (Exception)
      {
        return Response<DeletePostCommandResponse>.CreateErrorResponse("Invalid request");
      }

    }
  }
}
public class DeletePostCommandResponse
{
}
