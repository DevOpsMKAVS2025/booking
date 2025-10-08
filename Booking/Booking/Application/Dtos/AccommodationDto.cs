using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class AccommodationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ConvenieceType Convenience { get; set; }
        public List<string> Photos { get; set; }
        public int MinGuestNumber { get; set; }
        public int MaxGuestNumber { get; set; }
        public List<DateRange> Availability { get; set; }
        public List<Price> Prices { get; set; }
    }
}
