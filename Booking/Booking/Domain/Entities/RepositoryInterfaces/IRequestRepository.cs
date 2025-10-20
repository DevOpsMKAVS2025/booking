using Booking.BuildingBlocks.Core.UseCases;
using Booking.BuildingBlocks.Core;
using FluentResults;
using Booking.Domain.Entities.Booking.Domain.Entities;
using Booking.Application.Dtos;

namespace Booking.Domain.Entities.RepositoryInterfaces
{
    public interface IRequestRepository
    {
        PagedResult<Request> GetPaged(int page, int pageSize);
        Task<Request?> GetById(Guid id);
        Task<IEnumerable<Request>> GetByUserId(int userId);
        Task Create(Request request);
        Task Update(Request request);
        Task SaveChanges();
        Task<IEnumerable<Request>> GetByAccommodationAndUser(Guid accommodationId, int userId);
        Task<IEnumerable<Request>> GetByAccommodationId(Guid accommodationId);
        Task<IEnumerable<Request>> GetOverlappingRequests(Guid accommodationId, DateTime startDate, DateTime endDate, Guid excludeRequestId);
        Task<IEnumerable<RequestWithCancelCountDto>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId);
        Task<bool> HasOverlappingAcceptedRequest(Guid accommodationId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Request>> GetAcceptedByAccommodationId(Guid accommodationId);
        Task<IEnumerable<Request>> GetAcceptedByUserId(int userId);
        Task<IEnumerable<Request>> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId);
    }
}
