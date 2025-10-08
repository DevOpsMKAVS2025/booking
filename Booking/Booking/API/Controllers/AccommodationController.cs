using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Gym_tracker.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/accommodation")]
    public class AccommodationController : BaseApiController
    {
        private readonly IAccommodationService _accommodationService;

        public AccommodationController(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        [HttpGet]
        public IActionResult GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _accommodationService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _accommodationService.Get(id);
            return CreateResponse(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AccommodationDto accommodation)
        {
            var result = _accommodationService.Create(accommodation);
            return CreateResponse(result);
        }

        [HttpPut]
        public IActionResult Update([FromBody] AccommodationDto accommodation)
        {
            var result = _accommodationService.Update(accommodation);
            return CreateResponse(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _accommodationService.Delete(id);
            return CreateResponse(result);
        }
    }
}
