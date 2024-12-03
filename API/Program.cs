using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Tüm kaynaklara izin ver
              .AllowAnyHeader() // Tüm başlıklara izin ver
              .AllowAnyMethod(); // Tüm HTTP metodlarına izin ver
    });
});

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
app.UseCors("AllowAll");

app.MapPost("/users", ([FromBody] CreateUserCommand request, [FromServices] IMediator mediator) =>
{
    return mediator.Send(request);
})
.WithName("CreateUser");

app.Run();
