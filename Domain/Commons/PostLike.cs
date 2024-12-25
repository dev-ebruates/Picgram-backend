namespace Domain.Commons;

public class PostLike
{
  public Guid PostId { get; set; }
  public Post Post { get; set; }

  public Guid UserId { get; set; }
  public User User { get; set; }
}
