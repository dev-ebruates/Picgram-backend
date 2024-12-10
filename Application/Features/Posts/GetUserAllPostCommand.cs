namespace Application.Features.Posts;

public class GetUserAllPostCommand : IRequest<Response<List<GetUserAllPostCommandResponse>>>
{

  public GetUserAllPostCommand()
  {
  }

  public class GetUserAllPostCommandHandler : IRequestHandler<GetUserAllPostCommand, Response<List<GetUserAllPostCommandResponse>>>
  {
    readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GetUserAllPostCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<List<GetUserAllPostCommandResponse>>> Handle(GetUserAllPostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
       ?? throw new Exception("User not found");
        var userPosts = await unitOfWork.PostRepository.GetAllByUserId(Guid.Parse(userId));
        if (userPosts == null || userPosts.Count == 0)
          return Response<List<GetUserAllPostCommandResponse>>.CreateSuccessResponse(new List<GetUserAllPostCommandResponse>(0), "No posts found for the user");

        var postResponses = userPosts.Select(post => new GetUserAllPostCommandResponse(
          post.Id,
          post.User.Username,
          post.User.Bio,
          post.User.ProfilePicture,
          post.MediaUrl,
          post.Caption,
          post.CreatedAt
        )).OrderByDescending(x => x.CreatedAt).ToList();

        return Response<List<GetUserAllPostCommandResponse>>.CreateSuccessResponse(postResponses);
      }
      catch (Exception)
      {
        return Response<List<GetUserAllPostCommandResponse>>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class GetUserAllPostCommandResponse
{
  public Guid Id { get; set; }
  public string Username { get; set; } = null!;
  public string Bio { get; set; } = null!;
  public string? UserProfilePicture { get; set; }
  public string MediaUrl { get; set; } = null!;
  public string? Caption { get; set; }
  public DateTime CreatedAt { get; set; }

  public GetUserAllPostCommandResponse(Guid id, string username, string bio, string userProfilePicture, string mediaUrl, string? caption, DateTime createdAt)
  {
    Id = id;
    Username = username;
    Bio = bio;
    UserProfilePicture = userProfilePicture;
    MediaUrl = mediaUrl;
    Caption = caption;
    CreatedAt = createdAt;

  }
}

