using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Users;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string ProfilePicture { get; private set; }
    public string Bio { get; private set; }
    public List<Post> Posts { get; private set; }

    private User(string username, string email, string password, string profilePicture, string bio)
    {
        Username = username;
        Email = email;
        Password = password;
        ProfilePicture = profilePicture;
        Bio = bio;
        Posts = [];
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

        return new User(username, email, password, string.Empty, string.Empty);
    }

    public User AddPost(Post post)
    {
        Posts.Add(post);

        return this;
    }
}
