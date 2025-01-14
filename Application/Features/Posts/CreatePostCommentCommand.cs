namespace Application.Features.Posts;

public class CreatePostCommentCommand : IRequest<Response<PostCommentCommandResponse>>
{
  public string Comment { get; set; } = null!;
  public Guid PostId { get; set; }

  public class CreatePostCommentCommandHandler : IRequestHandler<CreatePostCommentCommand, Response<PostCommentCommandResponse>>
  {
    public readonly UnitOfWork unitOfWork;
    public readonly IHttpContextAccessor httpContextAccessor;
    public readonly SignalRUserService signalRUserService;


    public CreatePostCommentCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, SignalRUserService signalRUserService)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
      this.signalRUserService = signalRUserService;
    }

    public async Task<Response<PostCommentCommandResponse>> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(request.Comment))
          throw new ArgumentNullException(nameof(request.Comment));
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new Exception("User not found");
        var user = await unitOfWork.UserRepository.Get(Guid.Parse(userId)) ?? throw new Exception("User not found");
        var post = await unitOfWork.PostRepository.Get(request.PostId) ?? throw new Exception("Post not found");
        var postComment = new PostComment
        {
          Comment = request.Comment,
          PostId = request.PostId,
          UserId = Guid.Parse(userId)
        };
        post.AddComment(postComment);
        var notification = Notification.CommentNotification(user.Id, post.UserId, post.Id);
        unitOfWork.NotificationRepository.Add(notification);
        await unitOfWork.SaveChanges(cancellationToken);
        await signalRUserService.SendNotification(post.User.Username, "CommentPost");
        await signalRUserService.SendNotification(user.Username, "CommentPost");
        return Response<PostCommentCommandResponse>.CreateSuccessResponse(new PostCommentCommandResponse(
          postComment.Id,
          post.Id,
          postComment.Comment,
          user.Username,
          user.ProfilePicture,
          postComment.CreatedAt
        ));
      }
      catch (Exception)
      {
        return Response<PostCommentCommandResponse>.CreateErrorResponse("Invalid request");
      }

    }
  }
}
public class PostCommentCommandResponse
{
  public PostCommentCommandResponse(Guid id, Guid postId, string comment, string username, string? profilePicture, DateTime createdAt)
  {
    Id = id;
    PostId = postId;
    Comment = comment;
    Username = username;
    ProfilePicture = profilePicture;
    CreatedAt = createdAt;
  }

  public Guid Id { get; }
  public Guid PostId { get; }
  public string Comment { get; }
  public string Username { get; }
  public string? ProfilePicture { get; }
  public DateTime CreatedAt { get; }
}