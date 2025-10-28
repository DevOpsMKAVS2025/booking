using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class RequestDto
    {
        public Guid Id { get; set; }
        public Guid AccommodationId { get; set; }
        public string Accommodation { get; set; }
        public Guid GuestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestNum { get; set; }
    }
}
