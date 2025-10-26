using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class AccommodationWithRatingsDto
    {
        public string AccommodationId { get; set; }
        public string AccommodationName { get; set; }
        public string Average { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
