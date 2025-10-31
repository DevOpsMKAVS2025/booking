using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Database
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using BookingDbContext context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            context.Database.Migrate();
        }
    }
}
