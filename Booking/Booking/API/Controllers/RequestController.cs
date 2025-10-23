using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FluentResults;

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

        // ------------------ CREATE ------------------
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RequestDto dto)
        {
            var result = await _requestService.CreateRequest(dto); // Result<RequestDto>
            return CreateResponse(result);
        }

        // ------------------ GET ------------------
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var result = await _requestService.GetRequestById(id); // Result<RequestDto>
            return CreateResponse(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetByUser(int userId)
        {
            var result = await _requestService.GetRequestsByUser(userId); // Result<IEnumerable<RequestDto>>
            return CreateResponse(result);
        }

        [HttpGet("accommodation/{accommodationId}")]
        public async Task<ActionResult> GetByAccommodation(Guid accommodationId)
        {
            var result = await _requestService.GetRequestsByAccommodation(accommodationId); // Result<IEnumerable<RequestDto>>
            return CreateResponse(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetByAccommodationAndUser([FromQuery] Guid accommodationId, [FromQuery] int userId)
        {
            var result = await _requestService.GetByAccommodationAndUser(accommodationId, userId);
            return CreateResponse(result);
        }

        [HttpGet("paged")]
        public ActionResult GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _requestService.GetPaged(page, pageSize); // Result<PagedResult<RequestDto>>
            return CreateResponse(result);
        }

        // ------------------ APPROVE / REJECT ------------------
        [HttpPost("approve/{requestId}")]
        public async Task<ActionResult> Approve(Guid requestId)
        {
            var result = await _requestService.ApproveRequest(requestId); // Result<RequestDto>
            return CreateResponse(result);
        }

        [HttpPost("reject/{requestId}")]
        public async Task<ActionResult> Reject(Guid requestId)
        {
            var result = await _requestService.RejectRequest(requestId); // Result<RequestDto>
            return CreateResponse(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _requestService.DeleteRequest(id); // Result
            return CreateResponse(result);
        }

        // ------------------ EXTRA ------------------
        [HttpGet("accommodation/{accommodationId}/with-cancel-count")]
        public async Task<ActionResult> GetWithCancelCount(Guid accommodationId)
        {
            var result = await _requestService.GetRequestsWithCancelCountByAccommodation(accommodationId); // Result<IEnumerable<RequestWithCancelCountDto>>
            return CreateResponse(result);
        }

        [HttpGet("accepted/accommodation/{accommodationId}")]
        public async Task<ActionResult> GetAcceptedByAccommodation(Guid accommodationId)
        {
            var result = await _requestService.GetAcceptedByAccommodationId(accommodationId); // Result<IEnumerable<RequestDto>>
            return CreateResponse(result);
        }

        [HttpGet("accepted/user/{userId}")]
        public async Task<ActionResult> GetAcceptedByUser(int userId)
        {
            var result = await _requestService.GetAcceptedByUserId(userId); // Result<IEnumerable<RequestDto>>
            return CreateResponse(result);
        }

        [HttpGet("accepted/accommodation/{accommodationId}/user/{userId}")]
        public async Task<ActionResult> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId)
        {
            var result = await _requestService.GetAcceptedByAccommodationAndUser(accommodationId, userId); // Result<IEnumerable<RequestDto>>
            return CreateResponse(result);
        }
    }
}
