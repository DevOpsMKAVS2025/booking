using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Database.Repositories
{
    public class AccommodationRepository : IAccommodationRepository
    {

        protected readonly BookingDbContext DbContext;
        private readonly DbSet<Accommodation> _dbSet;

        public AccommodationRepository(BookingDbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Accommodation>();
        }

        public PagedResult<Accommodation> GetPaged(int page, int pageSize)
        {
            var query = _dbSet
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Id);

            int totalCount = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Accommodation>(items, totalCount);
        }


        public Accommodation Get(Guid id)
        {
            Accommodation entity = _dbSet
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefault();
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            return entity;
        }

        public Accommodation Create(Accommodation entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public Accommodation Update(Accommodation entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new KeyNotFoundException(e.Message);
            }
            return entity;
        }

        public void Delete(Guid id)
        {
            Accommodation entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public Price CreatePrice(Price price)
        {
            Accommodation accommodation = Get(price.AccommodationId);
            accommodation.Prices.Add(price);
            Update(accommodation);
            return price;
        }

        public Price UpdatePrice(Price price)
        {
            Price oldPrice = GetPrice(price.AccommodationId, price.Id);
            Accommodation accommodation = Get(price.AccommodationId);
            accommodation.Prices.Remove(oldPrice);
            accommodation.Prices.Add(price);
            Update(accommodation);
            return price;
        }

        public Price GetPrice(Guid accommodationId, Guid priceId)
        {
            Accommodation accommodation = Get(accommodationId);
            return accommodation.Prices.Find(x=> x.Id == priceId);  
        }

        public Availability CreateAvailability(Availability availability)
        {
            Accommodation accommodation = Get(availability.AccommodationId);
            accommodation.Availability.Add(availability);
            Update(accommodation);
            return availability;
        }

        public Availability UpdateAvailability(Availability availability)
        {
            Availability oldAvailability = GetAvailability(availability.AccommodationId, availability.Id);
            Accommodation accommodation = Get(availability.AccommodationId);
            accommodation.Availability.Remove(oldAvailability);
            accommodation.Availability.Add(availability);
            Update(accommodation);
            return availability;
        }

        public Availability GetAvailability(Guid accommodationId, Guid availabilityId)
        {
            Accommodation accommodation = Get(accommodationId);
            return accommodation.Availability.Find(x => x.Id == availabilityId);
        }

        public List<Accommodation> GetByOwnerId(Guid id)
        {
            return _dbSet.Where(x => x.OwnerId==id && !x.IsDeleted).ToList();
        }

        public async Task DeleteAccommodationsByOwnerId(Guid id)
        {
            await _dbSet
                .Where(a => a.OwnerId == id)
                .ExecuteUpdateAsync(a => a
                    .SetProperty(p => p.IsDeleted, p => true)
                );
        }

        public List<Accommodation> GetAll()
        {
            return _dbSet.ToList();
        }
    }
}
