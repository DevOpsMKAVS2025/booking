namespace Booking.Domain.Entities
{
    public class Availability
    {
        public Guid Id { get; set; }
        public Guid AccommodationId { get; set; }
        public DateRange Duration { get; set; }
        public Availability() { }

        public Availability(Guid id, Guid accommodationId, DateRange dateRange)
        {
            Id = id;
            AccommodationId = accommodationId;
            Duration = dateRange;
        }
    }
}
