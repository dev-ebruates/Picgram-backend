var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddApplication()
.AddInfrastructure()
.AddOpenApi();

var app = builder.Build();

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
