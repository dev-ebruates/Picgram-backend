namespace Domain.Users;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string ProfilePicture { get; private set; }
    public string Bio { get; private set; }

    private User(string username, string email, string password, string profilePicture, string bio)
    {
        Username = username;
        Email = email;
        Password = password;
        ProfilePicture = profilePicture;
        Bio = bio;
    }

    public static User Create(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) && username.Length < 3)
        {
            throw new ArgumentNullException("username");
        }
        if (string.IsNullOrWhiteSpace(email) && email.Contains("@"))
        {
            throw new ArgumentNullException("email");
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException("password");
        }

        return new User(username, email, password, string.Empty, string.Empty);
    }
}
