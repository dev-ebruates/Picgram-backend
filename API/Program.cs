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

var jwtSettings = builder.Configuration.GetSection("JwtSettings") ?? throw new Exception("JwtSettings not found");
var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("SecretKey not found");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<PicgramDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/users",
    ([FromBody] CreateUserCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("CreateUser");

app.MapPost("/auth",
    ([FromBody] AuthCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("Auth");

app.MapPost("/posts",
    ([FromBody] CreatePostCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("CreatePost")
.RequireAuthorization();

app.MapGet("/posts",
    ([FromServices] IMediator mediator) => mediator.Send(new GetAllPostCommand()))
.WithName("GetAllPost")
.RequireAuthorization();

app.MapGet("/user-posts",
    ([FromServices] IMediator mediator) => mediator.Send(new GetUserAllPostCommand()))
.WithName("GetUserAllPost")
.RequireAuthorization();

app.Run();
