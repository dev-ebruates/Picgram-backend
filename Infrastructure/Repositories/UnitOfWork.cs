namespace Infrastructure.Repositories;

public class UnitOfWork
{
  private readonly PicgramDbContext context;
  public IUserRepository UserRepository { get; private set; }
  public IPostRepository PostRepository { get; private set; }

  public UnitOfWork(PicgramDbContext context,
  IUserRepository userRepository,
  IPostRepository postRepository)
  {
    this.context = context;
    UserRepository = userRepository;
    PostRepository = postRepository;
  }

  public Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
  {
    return context.SaveChangesAsync(cancellationToken);
  }
}
