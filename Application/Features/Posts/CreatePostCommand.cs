using Microsoft.AspNetCore.Http;

namespace Application.Features.Posts;

public class CreatePostCommand : IRequest<Response<CreatePostCommandResponse>>
{
  public string MediaUrl { get; set; } = null!;
  public string? Caption { get; set; }


  public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Response<CreatePostCommandResponse>>
  {
    readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CreatePostCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }
    public async Task<Response<CreatePostCommandResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("User not found");
        var post = Post.Create(request.MediaUrl, request.Caption, Guid.Parse(userId));
        unitOfWork.PostRepository.Add(post);
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<CreatePostCommandResponse>.CreateSuccessResponse(new CreatePostCommandResponse(post.Id, post.UserId, post.MediaUrl, post.Caption), "Post created successfully");
      }
      catch (Exception)
      {
        return Response<CreatePostCommandResponse>.CreateErrorResponse("Invalid request");
      }
    }
  }
}
public class CreatePostCommandResponse
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string MediaUrl { get; set; }
  public string? Caption { get; set; }

  public CreatePostCommandResponse(Guid id, Guid userId, string mediaUrl, string? caption)
  {
    Id = id;
    UserId = userId;
    MediaUrl = mediaUrl;
    Caption = caption;
  }
}
