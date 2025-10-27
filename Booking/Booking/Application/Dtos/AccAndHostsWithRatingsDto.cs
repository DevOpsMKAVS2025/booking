namespace Booking.Application.Dtos
{
    public class AccAndHostsWithRatingsDto
    {
        public List<AccommodationWithRatingsDto> Accommodations { get; set; }
        public List<HostWithRatingsDto> Hosts { get; set; }
    }
}
