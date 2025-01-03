namespace Application.Features.Posts;

public class GetAllPostCommand : IRequest<Response<List<GetAllPostCommandResponse>>>
{
  public class GetAllPostCommandHandler : IRequestHandler<GetAllPostCommand, Response<List<GetAllPostCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GetAllPostCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<List<GetAllPostCommandResponse>>> Handle(GetAllPostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var posts = await unitOfWork.PostRepository.GetAll();
        if (posts == null || posts.Count == 0)
          return Response<List<GetAllPostCommandResponse>>.CreateSuccessResponse(new List<GetAllPostCommandResponse>(0), "No posts found");
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? throw new Exception("User not found");
        var postResponses = posts
          .Select(post =>
            new GetAllPostCommandResponse(
              post.Id,
              post.User.Username,
              post.User.ProfilePicture,
              post.MediaUrl,
              post.Caption,
              post.IsLiked(Guid.Parse(userId)),
              post.LikeCount,
              post.CreatedAt,
              post.Comments
                .Select(comment =>
                  new GetAllPostCommandResponse.PostCommentResponse(comment.Id,
                    comment.Comment,
                    comment.User.Username,
                    comment.User.ProfilePicture,
                    comment.CreatedAt))
                    .ToList()))
          .OrderByDescending(x => x.CreatedAt)
          .ToList();
        return Response<List<GetAllPostCommandResponse>>.CreateSuccessResponse(postResponses);
      }
      catch (Exception)
      {
        return Response<List<GetAllPostCommandResponse>>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class GetAllPostCommandResponse
{
  public Guid Id { get; set; }
  public string Username { get; set; }
  public string? UserProfilePicture { get; set; }
  public string MediaUrl { get; set; }
  public string? Caption { get; set; }
  public bool IsLiked { get; set; }
  public int LikeCount { get; set; }
  public DateTime CreatedAt { get; set; }
  public List<PostCommentResponse> Comments { get; set; }

  public GetAllPostCommandResponse(Guid id, string username, string? userProfilePicture, string mediaUrl, string? caption, bool isLiked, int likeCount, DateTime createdAt, List<PostCommentResponse> comments)
  {
    Id = id;
    Username = username;
    UserProfilePicture = userProfilePicture;
    MediaUrl = mediaUrl;
    Caption = caption;
    IsLiked = isLiked;
    LikeCount = likeCount;
    CreatedAt = createdAt;
    Comments = comments;
  }

  public class PostCommentResponse
  {
    public PostCommentResponse(Guid id, string comment, string username, string? profilePicture, DateTime createdAt)
    {
      Id = id;
      Comment = comment;
      Username = username;
      ProfilePicture = profilePicture;
      CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Comment { get; }
    public string Username { get; }
    public string? ProfilePicture { get; }
    public DateTime CreatedAt { get; }
  }
}
