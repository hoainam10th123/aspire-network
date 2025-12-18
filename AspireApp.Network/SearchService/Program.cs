using SearchService.Data;
using SearchService.MessageHandlers;
using Typesense;
using Typesense.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Add services to the container.

builder.Services.AddControllers();

// services__typesense__typesense__0 open searchService khi app host chay xem 
var typesenseUri = builder.Configuration["services:typesense:typesense:0"];

if (string.IsNullOrEmpty(typesenseUri))
    throw new InvalidOperationException("typesense uri not found");

var uri = new Uri(typesenseUri);

builder.Services.AddTypesenseClient(options =>
{
    options.ApiKey = "abc";
    options.Nodes = new List<Node>
    {
        new Node(uri.Host, uri.Port.ToString(), uri.Scheme)
    };
});

builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddHostedService<PostCreateHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchInitializer.EnsureIndexExists(client);

app.Run();
