
using System.Diagnostics.Contracts;

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

  public Task<Post?> Get(Guid id)
  {
    return context.Posts
    .Include(x => x.Likes)
    .Include(x => x.User)
    .Include(x => x.Comments)
    .ThenInclude(x => x.User)
    .Where(post => post.Id == id)
    .FirstOrDefaultAsync();
  }

  public Task<List<Post>> GetAll()
  {
    return context.Posts
    .Include(x => x.User)
    .Include(x => x.Likes)
    .Include(x => x.Comments)
    .ThenInclude(x => x.User)
    .ToListAsync();
  }

  public Task<List<Post>> GetAllByUserId(Guid userId)
  {
    return context.Posts.Include(x => x.User).Where(x => x.UserId == userId).ToListAsync();
  }
  public Task<List<Post>> GetAllByUsername(string username)
  {
    return context.Posts.Include(x => x.User).Where(x => x.User.Username == username).ToListAsync();
  }

  public Task<List<DateTime>> GetCreatedAtList()
  {
    return context
      .Posts
      .Select(x => x.CreatedAt)
      .ToListAsync();
  }

  public Task<PostComment> GetPostCommentByPostId(Guid postId, Guid commentId)
  {
    return context.Posts.Include(x => x.Comments).Where(x => x.Id == postId).SelectMany(x => x.Comments).Where(x => x.Id == commentId).FirstOrDefaultAsync();

  }
}
