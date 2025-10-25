using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class RequestWithCancelCountDto
    {
        public Guid RequestId { get; set; }
        public Guid AccommodationId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestNum { get; set; }
        public string State { get; set; }
        public int PreviousCancellations { get; set; }
    }
}
