namespace Domain.Messages;

public class Message : BaseEntity
{
  private Message(Guid senderId, Guid receiverId, string content)
  {
    SenderId = senderId;
    ReceiverId = receiverId;
    Content = content;
  }

  public static Message Create(Guid senderId, Guid receiverId, string content)
  {
    if (string.IsNullOrWhiteSpace(content))
      throw new ArgumentNullException(nameof(content));

    return new Message(senderId, receiverId, content);
  }

  public User Sender { get; private set; } = null!;
  public Guid SenderId { get; private set; }
  public User Receiver { get; private set; } = null!;
  public Guid ReceiverId { get; private set; }
  public string Content { get; private set; }
}
