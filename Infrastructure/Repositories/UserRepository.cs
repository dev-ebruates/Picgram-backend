﻿namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly PicgramDbContext context;

  public UserRepository(PicgramDbContext context)
  {
    this.context = context;
  }

  public User Add(User user)
  {
    context.Users.Add(user);
    return user;
  }
}
