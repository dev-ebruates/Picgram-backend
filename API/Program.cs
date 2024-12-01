using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddApplication()
.AddInfrastructure()
.AddOpenApi();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<PicgramDbContext>().Database.Migrate();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/users/create", ([FromBody] CreateUserCommand request, [FromServices] IMediator mediator) =>
{
    return mediator.Send(request);
})
.WithName("CreateUser");

app.Run();
