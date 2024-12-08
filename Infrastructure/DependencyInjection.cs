namespace Infrastructure;

public static class DependencyInjection
{

  public static IServiceCollection AddInfrastructure(this IServiceCollection services)
  {
    // var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Picgram.db");
    services.AddDbContextPool<PicgramDbContext>(options => options.UseSqlite($"Data Source=Picgram.db"));
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<UnitOfWork>();
    return services;
  }
}
