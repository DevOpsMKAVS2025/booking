using Booking.Infrastructure.Database;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Booking.Tests.IntegrationTests
{
    public class IntegrationTestBase : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _bookingDbContainer;
        private readonly PostgreSqlContainer _authDbContainer;
        private readonly RedisContainer _redisContainer;

        protected readonly HttpClient _httpClient;
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTestBase()
        {
            _bookingDbContainer = new PostgreSqlBuilder()
                .WithDatabase("booking-test-database")
                .WithUsername("postgres")
                .WithPassword("admin")
                .WithImage("postgres:15")
                .Build();

            _authDbContainer = new PostgreSqlBuilder()
                .WithDatabase("auth-test-database")
                .WithUsername("postgres")
                .WithPassword("admin")
                .WithImage("postgres:15")
                .Build();

            _redisContainer = new RedisBuilder()
                .WithImage("redis:7-alpine")
                .WithPortBinding(6379, true)
                .Build();

            // Start containers synchronously before configuring the WebApplicationFactory
            _bookingDbContainer.StartAsync().GetAwaiter().GetResult();
            _authDbContainer.StartAsync().GetAwaiter().GetResult();
            _redisContainer.StartAsync().GetAwaiter().GetResult();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        // Now safe to use GetConnectionString() since containers are started
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"Redis:ConnectionString", _redisContainer.GetConnectionString()}
                        });
                    });

                    builder.ConfigureServices(services =>
                    {
                        var bookingDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<BookingDbContext>));
                        if (bookingDescriptor != null)
                            services.Remove(bookingDescriptor);

                        var authDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AuthDBContext>));
                        if (authDescriptor != null)
                            services.Remove(authDescriptor);

                        services.AddDbContext<BookingDbContext>(options =>
                            options.UseNpgsql(_bookingDbContainer.GetConnectionString()));

                        services.AddDbContext<AuthDBContext>(options =>
                            options.UseNpgsql(_authDbContainer.GetConnectionString()));

                        // Configure Redis
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = _redisContainer.GetConnectionString();
                        });
                    });
                });

            _httpClient = _factory.CreateClient();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task InitializeAsync()
        {
            // Containers are already started in the constructor
            await ResetDatabaseAsync<BookingDbContext>();
            await ResetDatabaseAsync<AuthDBContext>();
        }

        public async Task DisposeAsync()
        {
            await _bookingDbContainer.DisposeAsync();
            await _authDbContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
            await _factory.DisposeAsync();
        }

        private async Task ResetDatabaseAsync<T>() where T : DbContext
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        protected async Task SeedDatabaseAsync<T>(Func<T, Task> seeder) where T : DbContext
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            await seeder(dbContext);
            await dbContext.SaveChangesAsync();
        }

        protected T GetDbContext<T>() where T : DbContext
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
