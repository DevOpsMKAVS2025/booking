using Booking.BuildingBlocks.Core;
using Booking.Domain.Entities.Booking.Domain.Entities;

namespace Booking.Domain.Entities
{
    public class Reservation : Entity
    {
        public Guid RequestId { get; set; }
        public Request Request { get; set; }
        public bool IsRejected { get; set; } = false;
    }
}
