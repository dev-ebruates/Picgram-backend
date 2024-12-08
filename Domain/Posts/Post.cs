namespace Domain.Posts;

public class Post : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string MediaUrl { get; private set; }
    public string? Caption { get; private set; }

    private Post(string mediaUrl, string? caption, Guid userId)
    {
        MediaUrl = mediaUrl;
        Caption = caption;
        UserId = userId;
    }
    
    public static Post Create(string mediaUrl, string? caption, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
        {
            throw new ArgumentNullException(nameof(mediaUrl));
        }
        if (string.IsNullOrWhiteSpace(caption))
        {
            throw new ArgumentNullException(nameof(caption));
        }   
        return new Post(mediaUrl, caption, userId);
    }
}

