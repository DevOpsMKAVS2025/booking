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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IImageService, ImageService>();
        }
        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IAccommodationRepository, AccommodationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();

            services.AddDbContext<BookingDbContext>(opt =>
            opt.UseNpgsql("Host=localhost;Port=5432;Database=booking-database;Username=postgres;Password=admin;IncludeErrorDetail=true",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "booking")));

            services.AddDbContext<AuthDBContext>(opt =>
            opt.UseNpgsql("Host=localhost;Port=5432;Database=devops;Username=admin;Password=admin;Pooling=true",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public")));
        }
    }
}
