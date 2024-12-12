
namespace Application.Features.Posts;

public class GetAllPostCommand : IRequest<Response<List<GetAllPostCommandResponse>>>
{
  public class GetAllPostCommandHandler : IRequestHandler<GetAllPostCommand, Response<List<GetAllPostCommandResponse>>>
  {
    readonly UnitOfWork unitOfWork;

    public GetAllPostCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetAllPostCommandResponse>>> Handle(GetAllPostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var posts = await unitOfWork.PostRepository.GetAll();
        if (posts == null || posts.Count == 0)
          return Response<List<GetAllPostCommandResponse>>.CreateSuccessResponse(new List<GetAllPostCommandResponse>(0), "No posts found");
        var postResponses = posts.Select(post => new GetAllPostCommandResponse(post.Id, post.User.Username, post.User.ProfilePicture, post.MediaUrl, post.Caption, post.CreatedAt)).OrderByDescending(x => x.CreatedAt).ToList();
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
  public string Username { get; set; } = null!;
  public string? UserProfilePicture { get; set; }
  public string MediaUrl { get; set; } = null!;
  public string? Caption { get; set; }
  public DateTime CreatedAt { get; set; }

  public GetAllPostCommandResponse(Guid id, string username, string? userProfilePicture, string mediaUrl, string? caption, DateTime createdAt)
  {
    Id = id;
    Username = username;
    UserProfilePicture = userProfilePicture;
    MediaUrl = mediaUrl;
    Caption = caption;
    CreatedAt = createdAt;
  }
}
