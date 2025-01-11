var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Tüm kaynaklara izin ver
              .AllowAnyHeader() // Tüm başlıklara izin ver
              .AllowAnyMethod(); // Tüm HTTP metodlarına izin ver
    });
    options.AddPolicy("AllowSignalR", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Frontend URL'nizi buraya ekleyin
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Kimlik bilgilerine izin ver
    });
});

builder.Services
.AddApplication()
.AddInfrastructure()
.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("JwtSettings") ?? throw new Exception("JwtSettings not found");
var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("SecretKey not found");

builder.Services.AddSignalR();
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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }

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

app.MapGet("/my-profile",
    ([FromServices] IMediator mediator) => mediator.Send(new GetMyProfileCommand()))
.WithName("GetMyProfile")
.RequireAuthorization();

app.MapGet("/profile/{username}",
   ([FromRoute] string username, [FromServices] IMediator mediator) => mediator.Send(new GetProfileCommand() { username = username }))
.WithName("GetProfile")
.RequireAuthorization();

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

app.MapGet("/user-posts/{username}",
    ([FromRoute] string username, [FromServices] IMediator mediator) => mediator.Send(new GetUserAllPostCommand { Username = username }))
.WithName("GetUserAllPost")
.RequireAuthorization();

app.MapPost("/user-bio",
    ([FromBody] UpdateUserBioCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("UpdateUserBio")
.RequireAuthorization();

app.MapPost("/user-profilePicture",
    ([FromBody] UpdateUserProfilePictureCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("UpdateUserProfilePicture")
.RequireAuthorization();

app.MapPost("/stories",
    ([FromBody] CreateStoryCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("CreateStory")
.RequireAuthorization();

app.MapGet("/stories/latest",
    ([FromServices] IMediator mediator) => mediator.Send(new GetAllLatestStoryCommand()))
.WithName("GetAllLatestStory")
.RequireAuthorization();

app.MapGet("/stories/{username}",
    ([FromRoute] string username, [FromServices] IMediator mediator) => mediator.Send(new GetAllLatestStoryByUserCommand { Username = username }))
.WithName("GetAllLatestStoryByUser")
.RequireAuthorization();

app.MapGet("/search/{searchParameter}",
    ([FromRoute] string searchParameter, [FromServices] IMediator mediator) => mediator.Send(new GetSearchCommand() { searchParameter = searchParameter }))
.WithName("GetSearch")
.RequireAuthorization();

app.MapPut("/posts/{id}/like",
    ([FromRoute] string id, [FromServices] IMediator mediator) => mediator.Send(new LikePostCommand { PostId = Guid.Parse(id) }))
.WithName("LikePost")
.RequireAuthorization();

app.MapPost("/posts/comment",
    ([FromBody] CreatePostCommentCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("PostComment")
.RequireAuthorization();

app.MapPost("/messages",
    ([FromBody] CreateMessageCommand request, [FromServices] IMediator mediator) => mediator.Send(request))
.WithName("CreateMessage")
.RequireAuthorization();

app.MapGet("/conversations",
    ([FromServices] IMediator mediator) => mediator.Send(new GetConversationsCommand()))
.WithName("GetConversations")
.RequireAuthorization();

app.MapGet("/relatedMessages/{senderUserId:guid}",
    ([FromRoute] Guid senderUserId, [FromServices] IMediator mediator) => mediator.Send(new GetRelatedMessagesCommand { ReceiverUserId = senderUserId }))
.WithName("GetRelatedMessages")
.RequireAuthorization();

app.MapGet("/notifications",
    ([FromServices] IMediator mediator) => mediator.Send(new GetAllNotificationByUserIdCommand()))
.WithName("GetAllNotificationByUserId")
.RequireAuthorization();

app.MapGet("/users",
    [Authorize(Roles = "Admin")] ([FromServices] IMediator mediator) => mediator.Send(new GetAllUsersCommand()))
.WithName("GetAllUsers")
.RequireAuthorization();

app.MapPut("/users/{id}/delete",
    ([FromRoute] string id, [FromServices] IMediator mediator) =>
        mediator.Send(new DeleteUserCommand { UserId = Guid.Parse(id) }))
.WithName("DeleteUser")
.RequireAuthorization();

app.MapGet("/getAllComments",
    [Authorize(Roles = "Admin")] ([FromServices] IMediator mediator) => mediator.Send(new GetAllCommentsCommand()))
.WithName("GetAllComments")
.RequireAuthorization();

app.MapPut("/posts/{postId:guid}/comments/{commentId:guid}",
    ([FromRoute] Guid postId, [FromRoute] Guid commentId, [FromServices] IMediator mediator) =>
        mediator.Send(new DeleteCommentCommand { PostId = postId, CommentId = commentId }))
.WithName("DeleteComment")
.RequireAuthorization();

app.MapPut("/posts/{id}/delete",
    ([FromRoute] string id, [FromServices] IMediator mediator) =>
        mediator.Send(new DeletePostCommand { PostId = Guid.Parse(id) }))
.WithName("DeletePost")
.RequireAuthorization();

app.MapGet("/send-notification/{message}", async (IHubContext<NotificationHub> hubContext, [FromRoute] string message) =>
{
    await hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    return Results.Ok();
});

app.MapHub<NotificationHub>("/notificationHub").RequireCors("AllowSignalR");

app.Run();
