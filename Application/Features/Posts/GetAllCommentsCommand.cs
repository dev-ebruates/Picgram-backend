using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Posts
{
  public class GetAllCommentsCommand : IRequest<Response<List<GetAllCommentsCommandResponse>>>
  {
  }

  public class GetAllCommentsCommandHandler : IRequestHandler<GetAllCommentsCommand, Response<List<GetAllCommentsCommandResponse>>>
  {
    private readonly UnitOfWork unitOfWork;

    public GetAllCommentsCommandHandler(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    public async Task<Response<List<GetAllCommentsCommandResponse>>> Handle(GetAllCommentsCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var posts = await unitOfWork.PostRepository.GetAll();
        if (posts == null || posts.Count == 0)
          return Response<List<GetAllCommentsCommandResponse>>.CreateSuccessResponse(new List<GetAllCommentsCommandResponse>(0), "No posts found");
        var postResponses = posts
          .Select(post =>
            new GetAllCommentsCommandResponse(
              post.Id,
              post.MediaUrl,
              post.CreatedAt,
              post.Comments
                .Select(comment =>
                  new GetAllCommentsCommandResponse.CommentResponse(comment.Id,
                    comment.Comment,
                    comment.User.Username,
                    comment.User.ProfilePicture,
                    comment.CreatedAt))
                    .ToList()))
          .OrderByDescending(x => x.CreatedAt)
          .ToList();
        return Response<List<GetAllCommentsCommandResponse>>.CreateSuccessResponse(postResponses);
      }
      catch (Exception)
      {
        return Response<List<GetAllCommentsCommandResponse>>.CreateErrorResponse("Invalid request");
      }
    }
  }

  public class GetAllCommentsCommandResponse
{
  public Guid Id { get; set; }
  public string MediaUrl { get; set; }
  public DateTime? CreatedAt { get; set;}
  public List<CommentResponse> Comments { get; set; }

  public GetAllCommentsCommandResponse(Guid id, string mediaUrl,DateTime createdAt, List<CommentResponse> comments)
  {
    Id = id;
    MediaUrl = mediaUrl;
    CreatedAt = createdAt;
    Comments = comments;
  }

    public class CommentResponse
    {
      public Guid Id { get; }
      public string Comment { get; }
      public string Username { get; }
      public string? ProfilePicture { get; }
      public DateTime CreatedAt { get; }

      public CommentResponse(Guid id, string comment, string username, string? profilePicture, DateTime createdAt)
      {
        Id = id;
        Comment = comment;
        Username = username;
        ProfilePicture = profilePicture;
        CreatedAt = createdAt;
      }
    }
  }
}
