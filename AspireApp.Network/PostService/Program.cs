using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PostService.Data;
using PostService.Mapper;
using PostService.Security;
using PostService.Service;
using SharedObject;



var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IKeycloakService, KeycloakService>();
builder.Services.AddMemoryCache();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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

//Cau hinh bai viet cua user nao thi user day moi duoc edit va xoa
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsPostOwner", policy =>
    {
        policy.Requirements.Add(new IsPostOwnerRequirement());
    });
});

builder.Services.AddTransient<IAuthorizationHandler, IsPostOwnerRequirementHandler>();

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

builder.AddNpgsqlDbContext<DataContext>("postdb");
builder.AddRabbitMQClient(connectionName: "messaging");
builder.Services.AddScoped<IBusRabbitmqService, BusRabbitmqService>();
builder.Services.AddAutoMapper(action =>
{
    action.AddProfile<AutoMapperProfiles>();
});

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
await dataContext.Database.MigrateAsync();
app.Run();
