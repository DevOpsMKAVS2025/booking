using Microsoft.EntityFrameworkCore;
using Booking.Infrastructure.Database;
using Booking;
using Booking.BuildingBlocks.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS_CONFIG", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));
builder.Services.AddControllers();
builder.Services.ConfigureBooking();
builder.Services.AddEndpointsApiExplorer();
var applyMigrations = builder.Configuration["Database:ApplyMigrations"] == "true";


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Booking API"
    });
});
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AuthDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDBString")));

string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
                               "redis:6379";

builder.Configuration["Redis:ConnectionString"] = redisConnectionString;


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "Booking_";
});
var port = builder.Configuration["Port"] ?? "5157";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});


var app = builder.Build();
if (applyMigrations) app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
    });
}

app.UseCors("CORS_CONFIG");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }