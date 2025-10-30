using Booking.BuildingBlocks.Core.UseCases;
using Booking.BuildingBlocks.Core;
using FluentResults;
using Booking.Domain.Entities;
using Booking.Application.Dtos;

namespace Booking.Domain.Entities.RepositoryInterfaces
{
    public interface IRequestRepository
    {
        PagedResult<Request> GetPaged(int page, int pageSize);
        Task<Request?> GetById(Guid id);
        Task<IEnumerable<Request>> GetByGuestId(Guid guestId);
        Task Create(Request request);
        Task Update(Request request);
        Task SaveChanges();
        Task<IEnumerable<Request>> GetByAccommodationAndGuest(Guid accommodationId, Guid guestId);
        Task<IEnumerable<Request>> GetByAccommodationId(Guid accommodationId);
        Task<IEnumerable<Request>> GetOverlappingRequests(Guid accommodationId, DateTime startDate, DateTime endDate, Guid excludeRequestId);
        Task<IEnumerable<RequestWithCancelCountDto>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId);
        Task<bool> HasOverlappingAcceptedRequest(Guid accommodationId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Request>> GetAcceptedByAccommodationId(Guid accommodationId);
        Task<IEnumerable<Request>> GetAcceptedByGuestId(Guid guestId);
        Task<IEnumerable<Request>> GetAcceptedByAccommodationAndGuest(Guid accommodationId, Guid guestId);
        List<Request> GetAllByGuestId(Guid guestId);
        List<Request> GetAllByAccommodationId(Guid accommodationId);
        List<Request> GetAllByHostId(Guid hostId);
        Task<bool> hasReservationsOwner(Guid ownerId);
        Task<bool> hasReservationsGuest(Guid guestId);
    }
}
