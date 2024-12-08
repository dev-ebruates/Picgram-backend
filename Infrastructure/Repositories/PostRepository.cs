namespace Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
  private readonly PicgramDbContext context;

  public PostRepository(PicgramDbContext context)
  {
    this.context = context;
  }

  public Post Add(Post post)
  {
    context.Posts.Add(post);
    return post;
  }
}
