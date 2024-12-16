
namespace Application.Features.Stories;

public class GetAllLatestStoryByUserCommand : IRequest<Response<List<GetAllLatestStoryByUserCommandResponse>>>
{
  public string Username { get; set; } = null!;

  public class GetAllLatestStoryByUserCommandHandler : IRequestHandler<GetAllLatestStoryByUserCommand, Response<List<GetAllLatestStoryByUserCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;

    public GetAllLatestStoryByUserCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetAllLatestStoryByUserCommandResponse>>> Handle(GetAllLatestStoryByUserCommand request, CancellationToken cancellationToken)
    {
      var stories = await unitOfWork.StoryRepository.GetAllLatestByUsername(request.Username);
      if (stories == null || stories.Count == 0)
      {
        return Response<List<GetAllLatestStoryByUserCommandResponse>>.CreateSuccessResponse(new List<GetAllLatestStoryByUserCommandResponse>(0), "No stories found");
      }

      return Response<List<GetAllLatestStoryByUserCommandResponse>>.CreateSuccessResponse(stories.Select(s => new GetAllLatestStoryByUserCommandResponse(s.MediaUrl, s.CreatedAt)).ToList());
    }
  }
}

public class GetAllLatestStoryByUserCommandResponse
{
  public string MediaUrl { get; set; }
  public DateTime CreatedAt { get; set; }

  public GetAllLatestStoryByUserCommandResponse(string mediaUrl, DateTime createdAt)
  {
    MediaUrl = mediaUrl;
    CreatedAt = createdAt;
  }
}