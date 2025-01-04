namespace Domain.Users;

public interface IUserRepository
{
  public User Add(User user);
  public Task<User?> GetByEmailOrUsername(string emailOrUsername);
  public Task<User?> Get(Guid id);
  public Task<User?> GetByUsername(string username);
  public Task<List<(Guid Id, string Username, string? ProfilePicture)>> GetAllSearch(string searchParameter);
  public Task<List<User>> GetAllByIds(List<Guid> ids, CancellationToken cancellationToken);
}
