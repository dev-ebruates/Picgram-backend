namespace Domain.Notifications;

public class Notification : BaseEntity
{
  private Notification(Guid triggerUserId, Guid userId, NotificationType type, Guid? postId)
  {
    TriggerUserId = triggerUserId;
    UserId = userId;
    Type = type;
    PostId = postId;
  }

  public static Notification LikedNotification(Guid triggerUserId, Guid userId, Guid? postId)
  {
    return new Notification(triggerUserId, userId, NotificationType.LikePost, postId);
  }

  public static Notification CommentNotification(Guid triggerUserId, Guid userId, Guid postId)
  {
    return new Notification(triggerUserId, userId, NotificationType.Comment, postId);
  }

  public Guid TriggerUserId { get; private set; }
  public User TriggerUser { get; private set; } = null!;
  public Guid UserId { get; private set; }
  public User User { get; private set; } = null!;
  public NotificationType Type { get; private set; }
  public Guid? PostId { get; private set; }
  public Post? Post { get; private set; }
}

public enum NotificationType
{
  LikePost = 1,
  Comment = 2,
}
