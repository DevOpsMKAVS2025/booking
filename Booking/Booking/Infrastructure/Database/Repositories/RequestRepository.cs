using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.BuildingBlocks.Infrastructure;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Booking.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        protected readonly BookingDbContext DbContext;
        private readonly DbSet<Request> _dbSet;

        public RequestRepository(BookingDbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Request>();
        }

        public PagedResult<Request> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public async Task<Request?> GetById(Guid id)
        {
            return await _dbSet
                .Include(r => r.Accommodation)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Request>> GetByGuestId(Guid guestId)
        {
            return await _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.GuestId == guestId
                    && r.State == RequestState.PENDING
                    && r.StartDate > DateTime.UtcNow
                    && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task Create(Request request)
        {
            await _dbSet.AddAsync(request);
        }

        public async Task Update(Request request)
        {
            _dbSet.Update(request);
        }

        public async Task SaveChanges()
        {
            await DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Request>> GetByAccommodationAndGuest(Guid accommodationId, Guid guestId)
        {
            return await _dbSet
                .Where(r => r.AccommodationId == accommodationId
                    && r.GuestId == guestId
                    && r.State == RequestState.PENDING
                    && r.StartDate > DateTime.UtcNow
                    && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByAccommodationId(Guid accommodationId)
        {
            return await _dbSet
                .Where(r => r.AccommodationId == accommodationId
                    && r.State == RequestState.PENDING
                    && r.StartDate > DateTime.UtcNow
                    && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetOverlappingRequests(Guid accommodationId, DateTime startDate, DateTime endDate, Guid excludeRequestId)
        {
            return await _dbSet
                .Where(r => r.AccommodationId == accommodationId
                            && r.Id != excludeRequestId
                            && r.State == RequestState.PENDING
                            && r.StartDate > DateTime.UtcNow
                            && !r.IsDeleted
                            && r.StartDate < endDate
                            && r.EndDate > startDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestWithCancelCountDto>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId)
        {
            return await _dbSet
                .Where(r => r.AccommodationId == accommodationId
                            && r.State == RequestState.PENDING
                            && r.StartDate > DateTime.UtcNow
                            && !r.IsDeleted)
                .Select(r => new RequestWithCancelCountDto
                {
                    RequestId = r.Id,
                    AccommodationId = r.AccommodationId,
                    GuestId = r.GuestId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    GuestNum = r.GuestNum,
                    State = r.State.ToString(),
                    PreviousCancellations = DbContext.Set<Request>()
                        .Count(res => res.GuestId == r.GuestId && res.State == RequestState.USER_REJECT)
                })
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingAcceptedRequest(Guid accommodationId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.AnyAsync(r => r.AccommodationId == accommodationId
                                             && r.State == RequestState.ACCEPTED
                                             && !r.IsDeleted
                                             && r.StartDate < endDate
                                             && r.EndDate > startDate);
        }

        public async Task<IEnumerable<Request>> GetAcceptedByAccommodationId(Guid accommodationId)
        {
            return await _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.AccommodationId == accommodationId
                            && r.State == RequestState.ACCEPTED
                            && !r.IsDeleted
                            && r.EndDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetAcceptedByGuestId(Guid guestId)
        {
            return await _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.GuestId == guestId
                            && r.State == RequestState.ACCEPTED
                            && !r.IsDeleted
                            && r.EndDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetAcceptedByAccommodationAndGuest(Guid accommodationId, Guid guestId)
        {
            return await _dbSet
                .Where(r => r.AccommodationId == accommodationId
                            && r.GuestId == guestId
                            && r.State == RequestState.ACCEPTED
                            && !r.IsDeleted
                            && r.EndDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public List<Request> GetAllByGuestId(Guid guestId)
        {
            return _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.GuestId == guestId && !r.IsDeleted)
                .ToList();
        }

        public List<Request> GetAllByAccommodationId(Guid accommodationId)
        {
            return _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.AccommodationId == accommodationId && !r.IsDeleted)
                .ToList();
        }

        public List<Request> GetAllByHostId(Guid hostId)
        {
            return _dbSet
                .Include(r => r.Accommodation)
                .Where(r => r.Accommodation.OwnerId == hostId && !r.IsDeleted)
                .ToList();
        }
    }
}
