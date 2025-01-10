namespace Application.Features.Posts;

public class DeleteCommentCommand : IRequest<Response<DeleteCommentCommandResponse>>
{
  public Guid CommentId { get; set; }
  public Guid PostId { get; set; }
  public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Response<DeleteCommentCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;

    public DeleteCommentCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<DeleteCommentCommandResponse>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
      try
      {
        // Kullanıcıyı silmek için repository'yi çağır
       var postComment = await unitOfWork.PostRepository.GetPostCommentByPostId(request.PostId, request.CommentId);
       postComment.Delete();
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<DeleteCommentCommandResponse>.CreateSuccessResponse();
      }
      catch (Exception ex)
      {
        return Response<DeleteCommentCommandResponse>.CreateErrorResponse("Invalid request");
      }

    }
  }
}
public class DeleteCommentCommandResponse
{
}
