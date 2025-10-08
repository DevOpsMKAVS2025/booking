namespace Booking.Domain.Entities
{
    public class Price
    {
        public decimal Amount { get; set; }
        public DateRange Duration { get; set; }
        public PriceType PriceType { get; set; }
    }

    public enum PriceType
    {
        PER_GUEST,
        PER_UNIT
    }
}
