using Booking.BuildingBlocks.Core;

namespace Booking.Domain.Entities
{
        public class Request : Entity
        {
            public Guid AccommodationId { get; set; }
            public Accommodation Accommodation { get; set; }
            public Guid GuestId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int GuestNum { get; set; }
            public RequestState State { get; set; }
            public bool IsDeleted { get; set; } = false;
        }

        public enum RequestState
        {
            PENDING,
            ACCEPTED,
            AUTO_REJECT,
            USER_REJECT
        }
}