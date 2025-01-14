﻿namespace Infrastructure.Repositories;

public class UnitOfWork
{
  private readonly PicgramDbContext context;
  public IUserRepository UserRepository { get; private set; }
  public IPostRepository PostRepository { get; private set; }
  public IStoryRepository StoryRepository { get; private set; }
  public IMessageRepository MessageRepository { get; private set; }
  public INotificationRepository NotificationRepository { get; private set; }

  public UnitOfWork(PicgramDbContext context,
  IUserRepository userRepository,
  IPostRepository postRepository,
  IStoryRepository storyRepository,
  IMessageRepository messageRepository,
  INotificationRepository notificationRepository)
  {
    this.context = context;
    UserRepository = userRepository;
    PostRepository = postRepository;
    StoryRepository = storyRepository;
    MessageRepository = messageRepository;
    NotificationRepository = notificationRepository;
  }

  public Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
  {
    return context.SaveChangesAsync(cancellationToken);
  }
}
