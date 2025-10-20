using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class RequestDto
    {
        public Guid AccommodationId { get; set; }
        public int UserId { get; set; } // mock value
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestNum { get; set; }
    }
}
