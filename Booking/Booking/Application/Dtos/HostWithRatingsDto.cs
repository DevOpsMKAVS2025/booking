using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class HostWithRatingsDto
    {
        public string HostId { get; set; }
        public string HostUsername { get; set; }
        public string Average { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
