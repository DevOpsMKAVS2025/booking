using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Database
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Accommodation> Accommodations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("booking"); 

            modelBuilder.Entity<Accommodation>()
                .OwnsMany(a => a.Availability, dr =>
                {
                    dr.ToTable("AccommodationAvailability");
                    dr.WithOwner().HasForeignKey("AccommodationId");
                });

            modelBuilder.Entity<Accommodation>()
                .OwnsMany(a => a.Prices, p =>
                {
                    p.ToTable("AccommodationPrices"); 
                    p.WithOwner().HasForeignKey("AccommodationId");
                    p.OwnsOne(x => x.Duration);
                });

        }
    }
}
