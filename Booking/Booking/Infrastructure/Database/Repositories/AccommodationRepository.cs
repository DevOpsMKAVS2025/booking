using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.BuildingBlocks.Core;
using Booking.BuildingBlocks.Infrastructure;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using FluentResults;
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
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Accommodation Get(Guid id)
        {
            var entity = _dbSet.Find(id);
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
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public Price CreatePrice(Price price)
        {
            var accommodation = Get(price.AccommodationId);
            accommodation.Prices.Add(price);
            Update(accommodation);
            return price;
        }

        public Price UpdatePrice(Price price)
        {
            var oldPrice = GetPrice(price.AccommodationId, price.Id);
            var accommodation = Get(price.AccommodationId);
            accommodation.Prices.Remove(oldPrice);
            accommodation.Prices.Add(price);
            Update(accommodation);
            return price;
        }

        public Price GetPrice(Guid accommodationId, Guid priceId)
        {
            var accommodation = Get(accommodationId);
            var price = accommodation.Prices.Find(x=> x.Id == priceId);  
            return price;
        }

        public Availability CreateAvailability(Availability availability)
        {
            var accommodation = Get(availability.AccommodationId);
            accommodation.Availability.Add(availability);
            Update(accommodation);
            return availability;
        }

        public Availability UpdateAvailability(Availability availability)
        {
            var oldAvailability = GetAvailability(availability.AccommodationId, availability.Id);
            var accommodation = Get(availability.AccommodationId);
            accommodation.Availability.Remove(oldAvailability);
            accommodation.Availability.Add(availability);
            Update(accommodation);
            return availability;
        }

        public Availability GetAvailability(Guid accommodationId, Guid availabilityId)
        {
            var accommodation = Get(accommodationId);
            var availability = accommodation.Availability.Find(x=>x.Id==availabilityId);
            return availability;
        }
    }
}
