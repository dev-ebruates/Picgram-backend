namespace Application.Features.Posts;

public class LikePostCommand : IRequest<Response<LikePostCommandResponse>>
{
  public Guid PostId { get; set; }

  public class LikePostCommandHandler : IRequestHandler<LikePostCommand, Response<LikePostCommandResponse>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly HttpUserService httpUserService;

    public LikePostCommandHandler(UnitOfWork unitOfWork, HttpUserService httpUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpUserService = httpUserService;
    }

    public async Task<Response<LikePostCommandResponse>> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
      var post = await unitOfWork.PostRepository.Get(request.PostId);
      if (post == null)
        return Response<LikePostCommandResponse>.CreateErrorResponse("Post not found");
      var userId = httpUserService.GetUserId();
      post.LikeUnlike(userId);
      if (post.IsLiked(userId))
      {
        var notification = Notification.LikedNotification(userId, post.UserId, post.Id);
        unitOfWork.NotificationRepository.Add(notification);
      }
      else
      {
        await unitOfWork.NotificationRepository.DeleteLikeNotification(userId, post.UserId, post.Id);
      }
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
