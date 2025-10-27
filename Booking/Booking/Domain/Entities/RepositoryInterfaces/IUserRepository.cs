namespace Booking.Domain.Entities.RepositoryInterfaces
{
    public interface IUserRepository
    {
        AppUser Get(Guid id);
    }
}
