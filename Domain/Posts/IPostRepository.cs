namespace Domain.Posts;

public interface IPostRepository
{
  public Post Add(Post post);
  public Task<List<Post>> GetAll();
  public Task<List<Post>> GetAllByUserId(Guid userId);
  public Task<Post?> Get(Guid id);
  public Task<List<Post>> GetAllByUsername(string username);
}
