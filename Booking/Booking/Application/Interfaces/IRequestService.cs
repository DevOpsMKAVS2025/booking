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
        Task<Result<IEnumerable<RequestDto>>> GetRequestsByUser(int userId);
        Task<Result<IEnumerable<RequestDto>>> GetRequestsByAccommodation(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetByAccommodationAndUser(Guid accommodationId, int userId);
        Result<PagedResult<RequestDto>> GetPaged(int page, int pageSize);
        Task<Result> DeleteRequest(Guid requestId);
        Task<Result<RequestDto>> ApproveRequest(Guid requestId);
        Task<Result<RequestDto>> RejectRequest(Guid requestId);
        Task<Result<IEnumerable<RequestWithCancelCountDto>>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationId(Guid accommodationId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByUserId(int userId);
        Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId);
    }
}
