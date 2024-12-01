namespace Infrastructure.Contexts;

public class PicgramDbContext : DbContext
{
  public PicgramDbContext(DbContextOptions<PicgramDbContext> options) : base(options)
  {

  }

  public DbSet<User> Users { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(x => x.Id).ValueGeneratedOnAdd();
      entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
      entity.Property(x => x.Password).IsRequired().HasMaxLength(255);
      entity.Property(x => x.Username).IsRequired().HasMaxLength(255);
      entity.HasIndex(x => x.Username).IsUnique();
      entity.HasIndex(x => x.Email).IsUnique();
    });
  }
}
