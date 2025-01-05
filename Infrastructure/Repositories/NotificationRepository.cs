namespace Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
  private readonly PicgramDbContext context;

  public NotificationRepository(PicgramDbContext context)
  {
    this.context = context;
  }

  public Notification Add(Notification notification)
  {
    context.Notifications.Add(notification);
    return notification;
  }

  public Task<List<Notification>> GetAllByUserId(Guid userId)
  {
    return context.Notifications
      .Include(x => x.TriggerUser)
      .Include(x => x.Post)
      .Where(x => x.UserId == userId).
      ToListAsync();
  }

  public async Task DeleteLikeNotification(Guid triggerUserId, Guid userId, Guid postId)
  {
    var notification = await context.Notifications
      .Where(x => x.Type == NotificationType.LikePost && x.TriggerUserId == triggerUserId && x.UserId == userId && x.PostId == postId)
      .FirstOrDefaultAsync();
    if (notification != null)
      context.Notifications.Remove(notification);
  }
}
