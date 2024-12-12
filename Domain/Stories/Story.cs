namespace Domain.Stories;

public class Story : BaseEntity
{
public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string MediaUrl { get; private set; }
   
    private Story(string mediaUrl, Guid userId)
    {
        MediaUrl = mediaUrl;
        UserId = userId;
    }
    
    public static Story Create(string mediaUrl, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
        {
            throw new ArgumentNullException(nameof(mediaUrl));
        }
        return new Story(mediaUrl, userId);
    }
}
