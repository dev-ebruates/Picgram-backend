namespace Application.Services;

public class HttpUserService
{
  private readonly IHttpContextAccessor httpContextAccessor;

  public HttpUserService(IHttpContextAccessor httpContextAccessor)
  {
    this.httpContextAccessor = httpContextAccessor;
  }

  public Guid GetUserId()
  {
    var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? throw new Exception("User not found");
    var userGuid = Guid.Parse(userId);
    return userGuid;
  }
}
