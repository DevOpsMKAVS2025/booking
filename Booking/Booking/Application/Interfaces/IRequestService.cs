using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.Booking.Domain.Entities;
using FluentResults;

namespace Booking.Application.Interfaces
{
    public interface IRequestService
    {
        public Result<PagedResult<RequestDto>> GetPaged(int page, int pageSize);
        Task<Request> CreateRequest(Request request);
        Task<bool> DeleteRequest(Guid requestId);
        Task<IEnumerable<Request>> GetRequestsByUser(int userId);
        Task<IEnumerable<Request>> GetByAccommodationAndUser(Guid accommodationId, int userId);
        Task<IEnumerable<Request>> GetRequestsByAccommodation(Guid accommodationId);
        Task<Request> ApproveRequest(Guid requestId);
        Task<Request> RejectRequest(Guid requestId);
        Task<IEnumerable<RequestWithCancelCountDto>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId);
        Task<IEnumerable<RequestDto>> GetAcceptedByAccommodationId(Guid accommodationId);
        Task<IEnumerable<RequestDto>> GetAcceptedByUserId(int userId);
        Task<IEnumerable<RequestDto>> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId);
    }
}
