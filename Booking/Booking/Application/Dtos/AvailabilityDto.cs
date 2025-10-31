using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class AvailabilityDto
    {
        public Guid? Id { get; set; }
        public Guid AccommodationId { get; set; }
        public DateRange Duration { get; set; }
    }
}
