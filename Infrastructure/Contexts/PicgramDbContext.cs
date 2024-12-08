namespace Infrastructure.Contexts;

public class PicgramDbContext : DbContext
{
  public PicgramDbContext(DbContextOptions<PicgramDbContext> options) : base(options)
  {

  }

  public DbSet<User> Users { get; private set; }
  public DbSet<Post> Posts { get; private set; }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    foreach (var entry in ChangeTracker.Entries())
    {
      if (entry.State == EntityState.Added)
        if (entry.Entity is BaseEntity entityAdded)
          entityAdded.Created();
      else if (entry.State == EntityState.Modified)
        if (entry.Entity is BaseEntity entityModified)
          entityModified.Updated();
    }

    return base.SaveChangesAsync(cancellationToken);
  }

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

     modelBuilder.Entity<Post>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(x => x.Id).ValueGeneratedOnAdd();
      entity.Property(x => x.MediaUrl).IsRequired();
      entity.Property(x => x.Caption).HasMaxLength(5000);
    });
  }
}
