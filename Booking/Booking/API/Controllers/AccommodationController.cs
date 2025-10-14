using Booking.Application.Dtos;
using Booking.Application.Interfaces;
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
        [HttpPost("price")]
        public IActionResult CreatePrice([FromBody] PriceDto price)
        {
            var result = _accommodationService.CreatePrice(price);
            return CreateResponse(result);
        }

        [HttpGet("price/{accomodationId}/{priceId}")]
        public IActionResult GetPrice(Guid priceId, Guid accomodationId)
        {
            var result = _accommodationService.GetPrice(accomodationId, priceId);
            return CreateResponse(result);
        }

        [HttpPut("price")]
        public IActionResult UpdatePrice([FromBody] PriceDto price)
        {
            var result = _accommodationService.UpdatePrice(price);
            return CreateResponse(result);
        }
        [HttpPost("availability")]
        public IActionResult CreateAvailability([FromBody] AvailabilityDto availability)
        {
            var result = _accommodationService.CreateAvailability(availability);
            return CreateResponse(result);
        }

        [HttpGet("availability/{accomodationId}/{priceId}")]
        public IActionResult GetAvailability(Guid availabilityId, Guid accomodationId)
        {
            var result = _accommodationService.GetAvailability(accomodationId, availabilityId);
            return CreateResponse(result);
        }

        [HttpPut("availability")]
        public IActionResult UpdateAvailability([FromBody] AvailabilityDto availability)
        {
            var result = _accommodationService.UpdateAvailability(availability);
            return CreateResponse(result);
        }
    }
}
