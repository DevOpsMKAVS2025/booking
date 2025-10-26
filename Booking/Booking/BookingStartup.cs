using Booking.Application;
using Booking.Application.Interfaces;
using Booking.Application.UseCases;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Booking.Infrastructure.Database;
using Booking.Infrastructure.Database.Repositories;
using Booking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking
{
    public static class BookingStartup
    {
        public static IServiceCollection ConfigureBooking(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BookingProfile));
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IAccommodationService, AccommodationService>();
            services.AddScoped<IRequestService, RequestService>();
        }
        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IAccommodationRepository, AccommodationRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();

            services.AddDbContext<BookingDbContext>(opt =>
            opt.UseNpgsql("Host=localhost;Port=5432;Database=booking-database;Username=postgres;Password=admin",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "booking")));
        }
    }
}
