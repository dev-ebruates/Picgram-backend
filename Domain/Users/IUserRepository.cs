﻿namespace Domain.Users;

public interface IUserRepository
{
  public User Add(User user);
  public Task<User?> GetByEmailOrUsername(string emailOrUsername);
  public Task<User?> Get(Guid id);

  public Task<List<(Guid Id, string Username)>> GetAllSearch(string searchParameter);
}
