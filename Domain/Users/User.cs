using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Users;

public class User : BaseEntity
{
    public const string PROFILE_PICTURE = "https://w7.pngwing.com/pngs/529/816/png-transparent-computer-icons-user-profile-avatar-heroes-monochrome-black-thumbnail.png";

    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string? ProfilePicture { get; private set; }
    public string? Bio { get; private set; }
    public UserRole Role { get; private set; }
    public List<Post> Posts { get; private set; }
    public List<PostLike> PostLikes { get; private set; }
    public List<PostComment> PostComments { get; private set; }

    public List<Message> SentMessages { get; set; }

    public List<Message> ReceivedMessages { get; set; }

    private User(string username, string email, string password, string profilePicture, string bio, UserRole role)
    {
        Username = username;
        Email = email;
        Password = password;
        ProfilePicture = profilePicture;
        Bio = bio;
        Role = role;
        Posts = [];
        PostLikes = [];
        PostComments = [];
        SentMessages = [];
        ReceivedMessages = [];
    }

    public static User Create(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) && username.Length < 2)
        {
            throw new ArgumentNullException(nameof(username));
        }
        if (string.IsNullOrWhiteSpace(email) && email.Contains('@'))
        {
            throw new ArgumentNullException(nameof(email));
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        return new User(username, email, password, PROFILE_PICTURE, string.Empty, UserRole.User);
    }

    public User AddPost(Post post)
    {
        Posts.Add(post);

        return this;
    }

    public User UpdateBio(string? bio)
    {
        Bio = bio;

        return this;
    }
    public User UpdateProfilePicture(string? profilePicture)
    {
        ProfilePicture = profilePicture;
        return this;
    }
}

public enum UserRole
{
    [Description("User")]
    User = 0,
    [Description("Admin")]
    Admin
}