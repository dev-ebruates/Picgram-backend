namespace Domain.Messages;

public interface IMessageRepository
{
  public Message Add(Message message);
  public Task<List<Guid>> GetConversations(Guid userId);
  public Task<List<Message>> GetRelatedMessages(Guid userId, Guid receiverId);
}
