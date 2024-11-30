namespace Domain.Commons;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Boolean IsDeleted { get; protected set; }

    public BaseEntity Delete()
    {
        IsDeleted = true;
        
        return this;
    }
}
