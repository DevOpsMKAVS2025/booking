using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.Application.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.Booking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/request")]
    public class RequestController : BaseApiController
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] RequestDto dto)
        {
            var request = new Request
            {
                AccommodationId = dto.AccommodationId,
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                GuestNum = dto.GuestNum
            };

            var created = await _requestService.CreateRequest(request);
            return CreatedAtAction(nameof(GetRequestById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            var requests = await _requestService.GetRequestsByUser(0);
            return Ok(requests);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            var success = await _requestService.DeleteRequest(id);
            if (!success)
                return BadRequest("Request not found or cannot be deleted.");

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRequests(int userId)
        {
            var requests = await _requestService.GetRequestsByUser(userId);
            return Ok(requests);
        }

        [HttpGet("paged")]
        public IActionResult GetPaged(int page = 1, int pageSize = 10)
        {
            var result = _requestService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetByAccommodationAndUser([FromQuery] Guid accommodationId, [FromQuery] int userId)
        {
            var requests = await _requestService.GetByAccommodationAndUser(accommodationId, userId);
            return Ok(requests);
        }

        [HttpGet("accommodation/{accommodationId}")]
        public async Task<IActionResult> GetRequestsByAccommodation(Guid accommodationId)
        {
            var requests = await _requestService.GetRequestsByAccommodation(accommodationId);
            return Ok(requests);
        }

        [HttpPost("approve/{requestId}")]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            try
            {
                var approvedRequest = await _requestService.ApproveRequest(requestId);
                return Ok(approvedRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reject/{requestId}")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            try
            {
                var rejectedRequest = await _requestService.RejectRequest(requestId);
                return Ok(rejectedRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("accommodation/{accommodationId}/with-cancel-count")]
        public async Task<IActionResult> GetRequestsWithCancelCount(Guid accommodationId)
        {
            var requests = await _requestService.GetRequestsWithCancelCountByAccommodation(accommodationId);
            return Ok(requests);
        }

        [HttpGet("accepted/accommodation/{accommodationId}")]
        public async Task<IActionResult> GetAcceptedByAccommodation(Guid accommodationId)
        {
            var requests = await _requestService.GetAcceptedByAccommodationId(accommodationId);
            return Ok(requests);
        }

        [HttpGet("accepted/user/{userId}")]
        public async Task<IActionResult> GetAcceptedByUser(int userId)
        {
            var requests = await _requestService.GetAcceptedByUserId(userId);
            return Ok(requests);
        }

        [HttpGet("accepted/accommodation/{accommodationId}/user/{userId}")]
        public async Task<IActionResult> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId)
        {
            var requests = await _requestService.GetAcceptedByAccommodationAndUser(accommodationId, userId);
            return Ok(requests);
        }

    }
}
