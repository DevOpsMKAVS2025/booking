using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class AccommodationDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ConvenieceType> Conveniences { get; set; }
        public List<string> Photos { get; set; }
        public int MinGuestNumber { get; set; }
        public int MaxGuestNumber { get; set; }
        public List<AvailabilityDto> Availability { get; set; }
        public List<PriceDto> Prices { get; set; }
        public decimal GlobalPrice { get; set; }
        public bool IsAutoReservation { get; set; }
    }
}
