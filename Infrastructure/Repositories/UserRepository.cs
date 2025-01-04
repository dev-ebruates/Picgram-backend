

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly PicgramDbContext context;

  public UserRepository(PicgramDbContext context)
  {
    this.context = context;
  }

  public User Add(User user)
  {
    context.Users.Add(user);
    return user;
  }

  public Task<User?> Get(Guid id)
  {
    return context.Users
    .Include(x => x.SentMessages)
    .Include(x => x.ReceivedMessages)
    .FirstOrDefaultAsync(x => x.Id == id);
  }

  public Task<User?> GetByEmailOrUsername(string emailOrUsername)
  {
    return context.Users.FirstOrDefaultAsync(x => x.Email == emailOrUsername || x.Username == emailOrUsername);
  }

  public async Task<List<(Guid Id, string Username, string? ProfilePicture)>> GetAllSearch(string searchParameter)
  {
    var users = await context.Users
    .Where(x => x.Username.ToLower().Contains(searchParameter.ToLower()))
    .Select(x => new { x.Id, x.Username, x.ProfilePicture })
    .ToListAsync();

    return users.Select(x => (x.Id, x.Username, x.ProfilePicture)).ToList();
  }

  public Task<User?> GetByUsername(string username)
  {
    return context.Users.FirstOrDefaultAsync(x => x.Username == username);
  }

  public Task<List<User>> GetAllByIds(List<Guid> ids, CancellationToken cancellationToken)
  {
    return context.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
  }
}
