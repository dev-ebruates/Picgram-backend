
namespace Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
  private readonly PicgramDbContext context;

  public MessageRepository(PicgramDbContext context)
  {
    this.context = context;
  }

  public Message Add(Message message)
  {
    context.Messages.Add(message);
    return message;
  }

  public async Task<List<Guid>> GetConversations(Guid userId)
  {
    return await context.Messages
      .Where(x => x.SenderId == userId || x.ReceiverId == userId)
      .Select(x => x.SenderId == userId ? x.ReceiverId : x.SenderId)
      .Distinct()
      .ToListAsync();
  }

  public Task<List<Message>> GetRelatedMessages(Guid primaryUserId, Guid seconderyUserId)
  {
    return context.Messages
      .Include(x => x.Sender)
      .Include(x => x.Receiver)
      .Where(x =>
        (x.SenderId == primaryUserId && x.ReceiverId == seconderyUserId) ||
        (x.SenderId == seconderyUserId && x.ReceiverId == primaryUserId))
      .OrderByDescending(x => x.CreatedAt)
      .ToListAsync();
  }
}
