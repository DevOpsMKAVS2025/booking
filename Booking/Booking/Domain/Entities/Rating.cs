using Booking.BuildingBlocks.Core;

namespace Booking.Domain.Entities
{
    public class Rating : Entity
    {
        public Guid AccommodationId { get; set; }
        public Guid HostId { get; set; }
        public Guid GuestId { get; set; }
        public int Evaluation { get; set; }
        public DateTime GivenAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
