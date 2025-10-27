using Booking.BuildingBlocks.Core;

namespace Booking.Domain.Entities
{
    public class Accommodation : Entity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ConvenieceType> Conveniences {  get; set; }
        public List<string> Photos { get; set; }
        public int MinGuestNumber { get; set; }
        public int MaxGuestNumber { get; set; }
        public List<Availability> Availability { get; set; }
        public List<Price> Prices { get; set; }
        public decimal GlobalPrice { get; set; }
        public bool IsAutoReservation { get; set; } = false;
        public PriceType PriceType { get; set; }
        public Guid OwnerId { get; set; }
    }

    public enum ConvenieceType
    {
        WIFI,
        KITCHEN,
        AIR_CONDITION,
        FREE_PARKING
    }
    public enum PriceType
    {
        PER_GUEST,
        PER_UNIT
    }
}
