namespace Booking.Domain.Entities
{
    public class Price
    {
        public Guid Id { get; set; }
        public Guid AccommodationId {  get; set; }
        public decimal Amount { get; set; }
        public DateRange Duration { get; set; }
    }
}
