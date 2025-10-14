using Booking.BuildingBlocks.Core.UseCases;
using Booking.BuildingBlocks.Core;
using FluentResults;

namespace Booking.Domain.Entities.RepositoryInterfaces
{
    public interface IAccommodationRepository
    {
        PagedResult<Accommodation> GetPaged(int page, int pageSize);
        Accommodation Get(Guid id);
        Accommodation Create(Accommodation entity);
        Accommodation Update(Accommodation entity);
        void Delete(Guid id);
        public Price CreatePrice(Price price);
        public Price UpdatePrice(Price price);
        public Price GetPrice(Guid accommodationId, Guid priceId);
        public Availability CreateAvailability(Availability availability);
        public Availability UpdateAvailability(Availability availability);
        public Availability GetAvailability(Guid accommodationId, Guid availabilityId);

    }
}
