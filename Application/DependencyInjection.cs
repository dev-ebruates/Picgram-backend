namespace Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services
    .AddMediatR(c => c.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly))
    .AddScoped<HttpUserService>()
    .AddSingleton<JwtTokenService>();
    
    return services;
  }
}
