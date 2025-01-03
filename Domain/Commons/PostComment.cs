namespace Domain.Commons;

public class PostComment : BaseEntity
{
  public Guid PostId { get; set; }
  public Post Post { get; set; }

  public Guid UserId { get; set; }
  public User User { get; set; }

  public string Comment { get; set; }
}
