namespace Application.Features.Stories;

public class CreateStoryCommand : IRequest<Response<CreateStoryCommandResponse>>
{
  public string MediaUrl { get; set; } = null!;



  public class CreateStoryCommandHandler : IRequestHandler<CreateStoryCommand, Response<CreateStoryCommandResponse>>
  {
    readonly UnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CreateStoryCommandHandler(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
    }
    public async Task<Response<CreateStoryCommandResponse>> Handle(CreateStoryCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("User not found");
        var story = Story.Create(request.MediaUrl, Guid.Parse(userId));
        unitOfWork.StoryRepository.Add(story);
        await unitOfWork.SaveChanges(cancellationToken);
        return Response<CreateStoryCommandResponse>.CreateSuccessResponse(new CreateStoryCommandResponse(story.Id, story.UserId, story.MediaUrl), "Story created successfully");
      }
      catch (Exception)
      {
        return Response<CreateStoryCommandResponse>.CreateErrorResponse("Invalid request");
      }
    }
  }
}
public class CreateStoryCommandResponse
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string MediaUrl { get; set; }


  public CreateStoryCommandResponse(Guid id, Guid userId, string mediaUrl)
  {
    Id = id;
    UserId = userId;
    MediaUrl = mediaUrl;
  }
}

