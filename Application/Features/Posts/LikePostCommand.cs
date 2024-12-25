namespace Application.Features.Posts;

public class LikePostCommand : IRequest<Response<LikePostCommandResponse>>
{
  public Guid PostId { get; set; }

  public class LikePostCommandHandler : IRequestHandler<LikePostCommand, Response<LikePostCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LikePostCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<LikePostCommandResponse>> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
      var post = await unitOfWork.PostRepository.Get(request.PostId);
      if (post == null)
        return Response<LikePostCommandResponse>.CreateErrorResponse("Post not found");
      var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("User not found");
      post.LikeUnlike(Guid.Parse(userId));
      await unitOfWork.SaveChanges(cancellationToken);
      return Response<LikePostCommandResponse>.CreateSuccessResponse(new LikePostCommandResponse(post.Id, post.LikeCount), "Post liked successfully");
    }
  }
}

public class LikePostCommandResponse
{
  public Guid PostId { get; set; }
  public int LikeCount { get; set; }

  public LikePostCommandResponse(Guid postId, int likeCount)
  {
    PostId = postId;
    LikeCount = likeCount;
  }
}
