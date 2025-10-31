namespace Booking.Application.Dtos
{
    public class NewRatingDto
    {
        public string Id { get; set; }
        public string type { get; set; }
        public string accommodationId { get; set; }
        public string hostId { get; set; }
        public string guestId { get; set; }
        public int evaluation { get; set; }
    }
}
