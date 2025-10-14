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
                    dr.ToTable("Accommodation_Availability");
                    dr.WithOwner().HasForeignKey("AccommodationId");
                    dr.OwnsOne(av => av.Duration);
                });

            modelBuilder.Entity<Accommodation>()
                .OwnsMany(a => a.Prices, p =>
                {
                    p.ToTable("Accommodation_Prices"); 
                    p.WithOwner().HasForeignKey("AccommodationId");
                    p.OwnsOne(x => x.Duration);
                });

        }
    }
}
