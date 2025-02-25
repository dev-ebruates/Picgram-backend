﻿namespace Application.Features.Posts;

public class GetUserAllPostCommand : IRequest<Response<List<GetUserAllPostCommandResponse>>>
{
  public string Username { get; set; } = null!;

  public GetUserAllPostCommand()
  {
  }

  public class GetUserAllPostCommandHandler : IRequestHandler<GetUserAllPostCommand, Response<List<GetUserAllPostCommandResponse>>>
  {
    readonly UnitOfWork unitOfWork;

    public GetUserAllPostCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetUserAllPostCommandResponse>>> Handle(GetUserAllPostCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var userPosts = await unitOfWork.PostRepository.GetAllByUsername(request.Username);
        if (userPosts == null || userPosts.Count == 0)
          return Response<List<GetUserAllPostCommandResponse>>.CreateSuccessResponse(new List<GetUserAllPostCommandResponse>(0), "No posts found for the user");

        var postResponses = userPosts.Select(post => new GetUserAllPostCommandResponse(
          post.Id,
          post.User.Username,
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
  public string? UserProfilePicture { get; set; }
  public string MediaUrl { get; set; } = null!;
  public string? Caption { get; set; }
  public DateTime CreatedAt { get; set; }

  public GetUserAllPostCommandResponse(Guid id, string username, string? userProfilePicture, string mediaUrl, string? caption, DateTime createdAt)
  {
    Id = id;
    Username = username;
    UserProfilePicture = userProfilePicture;
    MediaUrl = mediaUrl;
    Caption = caption;
    CreatedAt = createdAt;

  }
}

