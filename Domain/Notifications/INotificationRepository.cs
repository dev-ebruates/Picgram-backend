namespace Domain.Notifications;

public interface INotificationRepository
{
  Notification Add(Notification notification);
  Task<List<Notification>> GetAllByUserId(Guid userId);
  Task DeleteLikeNotification(Guid triggerUserId, Guid userId, Guid postId);
}
