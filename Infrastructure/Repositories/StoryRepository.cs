namespace Infrastructure.Repositories;

public class StoryRepository : IStoryRepository
{
  private readonly PicgramDbContext context;

  public StoryRepository(PicgramDbContext context)
  {
    this.context = context;
  }
  public Story Add(Story story)
  {
    context.Stories.Add(story);
    return story;
  }

  public Task<List<Story>> GetAllLatest()
  {
    return context.Stories
    .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-1))
    .Include(x => x.User)
    .ToListAsync();
  }
}
