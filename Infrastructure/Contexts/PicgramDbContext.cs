namespace Infrastructure.Contexts;

public class PicgramDbContext : DbContext
{
  public PicgramDbContext(DbContextOptions<PicgramDbContext> options) : base(options)
  {
    
  }

  public DbSet<User> Users { get; private set; }
  
}
