
using ChatService.MessageHandlers;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.AddServiceDefaults();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                      });
});
// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddAuthentication()
//    .AddKeycloakJwtBearer(serviceName: "keycloakauth", realm: "network", opt =>
//    {
//        opt.Audience = "network";
//        opt.RequireHttpsMetadata = false;
//        opt.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidIssuers =
//            [
//                "http://localhost:8080/realms/network"
//            ],
//            ClockSkew = TimeSpan.Zero,
//        };
//    });

builder.Services.AddSignalR();
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddHostedService<RealtimeCommentHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatService.Hubs.ChatHub>("hub/chat");

app.Run();
