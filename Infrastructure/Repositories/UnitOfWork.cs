namespace Infrastructure.Repositories;

public class UnitOfWork
{
  private readonly PicgramDbContext context;
  public IUserRepository UserRepository { get; private set; }

  public UnitOfWork(PicgramDbContext context, IUserRepository userRepository)
  {
    this.context = context;
    UserRepository = userRepository;
  }

  public Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
  {
    return context.SaveChangesAsync(cancellationToken);
  }
}
