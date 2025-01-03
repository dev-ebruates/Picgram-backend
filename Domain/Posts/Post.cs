namespace Domain.Posts;

public class Post : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string MediaUrl { get; private set; }
    public string? Caption { get; private set; }
    public List<PostLike> Likes { get; private set; }
    public List<PostComment> Comments { get; private set; }
    public int LikeCount => Likes.Count;

    private Post(string mediaUrl, string? caption, Guid userId)
    {
        MediaUrl = mediaUrl;
        Caption = caption;
        UserId = userId;
        Likes = [];
        Comments = [];
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

    public Post LikeUnlike(Guid userId)
    {
        if (Likes.Any(x => x.UserId == userId))
        {
            var postLike = Likes.FirstOrDefault(x => x.UserId == userId);
            if (postLike != null)
                Likes.Remove(postLike);
        }
        else
        {
            Likes.Add(new PostLike
            {
                UserId = userId,
                PostId = Id
            });
        }

        return this;
    }

    public bool IsLiked(Guid userId)
    {
        return Likes.Any(x => x.UserId == userId);
    }
    public Post AddComment(PostComment postComment)
    {
        Comments.Add(postComment);
        return this;
    }
}

