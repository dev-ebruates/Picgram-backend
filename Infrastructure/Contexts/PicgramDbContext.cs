namespace Infrastructure.Contexts;

public class PicgramDbContext : DbContext
{
  public PicgramDbContext(DbContextOptions<PicgramDbContext> options) : base(options)
  {

  }

  public DbSet<User> Users { get; private set; }
  public DbSet<Post> Posts { get; private set; }
  public DbSet<Story> Stories { get; private set; }

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
      entity.HasQueryFilter(e => !e.IsDeleted);
    });

    modelBuilder.Entity<Post>(entity =>
   {
     entity.HasKey(e => e.Id);
     entity.Property(x => x.Id).ValueGeneratedOnAdd();
     entity.Property(x => x.MediaUrl).IsRequired();
     entity.Property(x => x.Caption).HasMaxLength(5000);
     entity.Ignore(x => x.LikeCount);
     entity.HasQueryFilter(e => !e.IsDeleted);
   });

    modelBuilder.Entity<Story>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(x => x.Id).ValueGeneratedOnAdd();
      entity.Property(x => x.MediaUrl).IsRequired();
      entity.HasQueryFilter(e => !e.IsDeleted);
    });

    modelBuilder.Entity<PostLike>(entity =>
    {
      entity.HasKey(e => new { e.PostId, e.UserId });
      entity.HasOne(e => e.Post)
        .WithMany(e => e.Likes)
        .HasForeignKey(e => e.PostId);
      entity.HasOne(e => e.User)
        .WithMany(e => e.PostLikes)
        .HasForeignKey(e => e.UserId);
    });
  }
}
