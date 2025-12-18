using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomService.Data;
using RoomService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILivekitService, LivekitService>();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(serviceName: "keycloakauth", realm: "network", opt =>
    {
        opt.Audience = "network";
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuers =
            [
                "http://localhost:8080/realms/network"
            ],
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.AddNpgsqlDbContext<DataContext>("roomdb");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
await dataContext.Database.MigrateAsync();

app.Run();
