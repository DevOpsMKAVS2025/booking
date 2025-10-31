using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class PriceDto
    {
        public Guid? Id { get; set; }
        public Guid AccommodationId { get; set; }
        public decimal Amount { get; set; }
        public DateRange Duration { get; set; }
    }
}
