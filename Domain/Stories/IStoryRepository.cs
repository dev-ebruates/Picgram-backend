namespace Domain.Stories;

public interface IStoryRepository
{
  public Story Add(Story story);
  public Task<List<Story>> GetAllLatest();
}
