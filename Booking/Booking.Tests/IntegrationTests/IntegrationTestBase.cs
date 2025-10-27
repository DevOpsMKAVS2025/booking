using Booking.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Booking.Tests.IntegrationTests
{
    public class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly HttpClient _httpClient;
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;

        public IntegrationTestBase()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var sp = services.BuildServiceProvider();
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        // BookingDbContext
                        var bookingDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<BookingDbContext>));
                        if (bookingDescriptor != null)
                            services.Remove(bookingDescriptor);

                        services.AddDbContext<BookingDbContext>(options =>
                            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=test-booking-db"));

                        // AuthDbContext
                        var authDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AuthDBContext>));
                        if (authDescriptor != null)
                            services.Remove(authDescriptor);

                        services.AddDbContext<AuthDBContext>(options =>
                            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=test-auth-db"));
                    });
                });

            _httpClient = factory.CreateClient();
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // ✅ Reset svih baza pre svakog testa
        public async Task InitializeAsync()
        {
            await ResetDatabaseAsync<BookingDbContext>();
            await ResetDatabaseAsync<AuthDBContext>();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task ResetDatabaseAsync<T>() where T : DbContext
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        // ✅ Seed podataka u određeni DbContext
        protected async Task SeedDatabaseAsync<T>(Func<T, Task> seeder) where T : DbContext
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            await seeder(dbContext);
            await dbContext.SaveChangesAsync();
        }

        // ✅ Pristup kontekstu direktno iz testa
        protected T GetDbContext<T>() where T : DbContext
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
