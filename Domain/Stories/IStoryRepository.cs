namespace Domain.Stories;

public interface IStoryRepository
{
  public Story Add(Story story);
  public Task<List<Story>> GetAllLatest();
  public Task<List<Story>> GetAllLatestByUsername(string username);
}
