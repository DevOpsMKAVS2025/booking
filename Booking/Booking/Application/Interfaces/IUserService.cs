using Booking.Domain.Entities;

namespace Booking.Application.Interfaces
{
    public interface IUserService
    {
        AppUser Get(Guid id);
    }
}
