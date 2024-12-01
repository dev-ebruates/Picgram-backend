namespace Infrastructure.Repositories;

public class UnitOfWork
{
  public IUserRepository UserRepository { get; private set; }
  
  public UnitOfWork(IUserRepository userRepository)
  {
    UserRepository = userRepository;
  }
}
