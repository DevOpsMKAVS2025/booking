using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FluentResults;
using Booking.BuildingBlocks.Core.UseCases;

namespace Booking.API.Controllers
{
    [Route("api/request")]
    [ApiController]
    public class RequestController : BaseApiController
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RequestDto dto)
        {
            Result<RequestDto> result = await _requestService.CreateRequest(dto);
            return CreateResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            Result<RequestDto> result = await _requestService.GetRequestById(id);
            return CreateResponse(result);
        }

        [HttpGet("guest/{guestId}")]
        public async Task<ActionResult> GetByGuest(Guid guestId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetRequestsByGuest(guestId);
            return CreateResponse(result);
        }

        [HttpGet("accommodation/{accommodationId}")]
        public async Task<ActionResult> GetByAccommodation(Guid accommodationId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetRequestsByAccommodation(accommodationId);
            return CreateResponse(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetByAccommodationAndGuest([FromQuery] Guid accommodationId, [FromQuery] Guid guestId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetByAccommodationAndGuest(accommodationId, guestId);
            return CreateResponse(result);
        }

        [HttpGet("paged")]
        public ActionResult GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            Result<PagedResult<RequestDto>> result = _requestService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

        [HttpPost("approve/{requestId}")]
        public async Task<ActionResult> Approve(Guid requestId)
        {
            Result<RequestDto> result = await _requestService.ApproveRequest(requestId);
            return CreateResponse(result);
        }

        [HttpPost("reject/{requestId}")]
        public async Task<ActionResult> Reject(Guid requestId)
        {
            Result<RequestDto> result = await _requestService.RejectReservation(requestId);
            return CreateResponse(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            Result<RequestDto> result = await _requestService.DeleteRequest(id);
            return CreateResponse(result);
        }

        [HttpGet("accommodation/{accommodationId}/with-cancel-count")]
        public async Task<ActionResult> GetWithCancelCount(Guid accommodationId)
        {
            Result<IEnumerable<RequestWithCancelCountDto>> result = await _requestService.GetRequestsWithCancelCountByAccommodation(accommodationId);
            return CreateResponse(result);
        }

        [HttpGet("accepted/accommodation/{accommodationId}")]
        public async Task<ActionResult> GetAcceptedByAccommodation(Guid accommodationId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetAcceptedByAccommodationId(accommodationId);
            return CreateResponse(result);
        }

        [HttpGet("accepted/guest/{guestId}")]
        public async Task<ActionResult> GetAcceptedByGuest(Guid guestId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetAcceptedByGuestId(guestId);
            return CreateResponse(result);
        }

        [HttpGet("accepted/accommodation/{accommodationId}/guest/{guestId}")]
        public async Task<ActionResult> GetAcceptedByAccommodationAndGuest(Guid accommodationId, Guid guestId)
        {
            Result<IEnumerable<RequestDto>> result = await _requestService.GetAcceptedByAccommodationAndGuest(accommodationId, guestId);
            return CreateResponse(result);
        }

        [HttpGet("user/{userId}/has-reservations")]
        public async Task<ActionResult> HasReservationsOwner(Guid userId, [FromQuery] bool isOwner)
        {
            var result = await _requestService.hasReservations(userId, isOwner);
            return Ok(result);
        }
    }
}
