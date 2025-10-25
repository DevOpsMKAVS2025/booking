using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.Booking.Domain.Entities;
using FluentResults;

namespace Booking.Application.Interfaces
{
    public interface IRequestService
    {
        Task<Result<RequestDto>> CreateRequest(RequestDto dto);
        Task<Result<RequestDto>> GetRequestById(Guid requestId);
        Task<Result<IEnumerable<RequestDto>>> GetRequestsByGuest(Guid guestId);
        Task<Result<IEnumerable<RequestDto>>> GetRequestsByAccommodation(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetByAccommodationAndGuest(Guid accommodationId, Guid guestId);
        Result<PagedResult<RequestDto>> GetPaged(int page, int pageSize);
        Task<Result> DeleteRequest(Guid requestId);
        Task<Result<RequestDto>> ApproveRequest(Guid requestId);
        Task<Result<RequestDto>> RejectRequest(Guid requestId);
        Task<Result<IEnumerable<RequestWithCancelCountDto>>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationId(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByGuestId(Guid guestId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationAndGuest(Guid accommodationId, Guid guestId);
    }
}
