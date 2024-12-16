namespace Application.Features.Stories;

public class GetAllLatestStoryCommand : IRequest<Response<List<GetAllLatestStoryCommandResponse>>>
{
  public class GetAllStoryCommandHandler : IRequestHandler<GetAllLatestStoryCommand, Response<List<GetAllLatestStoryCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;

    public GetAllStoryCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetAllLatestStoryCommandResponse>>> Handle(GetAllLatestStoryCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var latestStories = await unitOfWork.StoryRepository.GetAllLatest();
        if (latestStories == null || latestStories.Count == 0)
          return Response<List<GetAllLatestStoryCommandResponse>>.CreateSuccessResponse(new List<GetAllLatestStoryCommandResponse>(0), "No stories found");
        var storyResponses = latestStories
        .GroupBy(s => new { s.User.Username, s.User.ProfilePicture })
        .Select(group => new GetAllLatestStoryCommandResponse(group.Key.Username, group.Key.ProfilePicture))
        .ToList();

        return Response<List<GetAllLatestStoryCommandResponse>>.CreateSuccessResponse(storyResponses);
      }
      catch (Exception)
      {
        return Response<List<GetAllLatestStoryCommandResponse>>.CreateErrorResponse("Invalid request");
      }
    }
  }
}

public class GetAllLatestStoryCommandResponse
{
  public string Username { get; set; }
  public string? UserProfilePicture { get; set; }

  public GetAllLatestStoryCommandResponse(string username, string? userProfilePicture)
  {
    Username = username;
    UserProfilePicture = userProfilePicture;
  }
}