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

  public Task<List<Story>> GetAll()
  {
    return context.Stories.Include(x => x.User).ToListAsync();
  }
}
