namespace Booking.Domain.Entities
{
    public class Accommodation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public CONVENIENCE_TYPE Convenience {  get; set; }
        public List<string> Photos { get; set; }
        public int MinGuestNumber { get; set; }
        public int MaxGuestNumber { get; set; }
    }

    public enum CONVENIENCE_TYPE
    {
        WIFI,
        KITCHEN,
        AIR_CONDITION,
        FREE_PARKING
    }
}
