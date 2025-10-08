using Booking.BuildingBlocks.Infrastructure;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Database.Repositories
{
    public class AccommodationRepository : CrudDatabaseRepository<Accommodation, BookingDbContext>
    {
        public AccommodationRepository(BookingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
