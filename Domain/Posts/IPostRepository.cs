namespace Domain.Posts;

public interface IPostRepository
{
  public Post Add(Post post);
  public Task<List<Post>> GetAll();
}
