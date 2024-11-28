namespace Domain;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; }
  public DateTime CreatedAt { get; protected set; }
}
