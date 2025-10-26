using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/rating")]
    public class RatingController : BaseApiController
    {
        private readonly IRatingService _ratingService;
        private readonly IAccommodationService _accommodationService;
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;

        public RatingController(IRatingService ratingService, IAccommodationService accommodationService, IRequestService requestService, IUserService userService)
        {
            _ratingService = ratingService;
            _accommodationService = accommodationService;
            _requestService = requestService;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult AddRatingForAccommodation([FromBody] NewRatingDto ratingDto)
        {
            if (ratingDto.evaluation < 1 || ratingDto.evaluation > 5) {
                return BadRequest("Evaluation must be between 1 and 5.");
            }

            Guid parsedGuestId = Guid.TryParse(ratingDto.guestId, out var g1) ? g1 : Guid.Empty;
            Guid parsedAccommodationId = Guid.TryParse(ratingDto.accommodationId, out var g2) ? g2 : Guid.Empty;
            Guid parsedHostId = Guid.TryParse(ratingDto.hostId, out var g3) ? g3 : Guid.Empty;

            var rating = new Rating
            {
                Evaluation = ratingDto.evaluation,
                GuestId = parsedGuestId,
                GivenAt = DateTime.UtcNow,
            };

            var requests = _requestService.GetAllByGuestId(parsedGuestId);

            if (ratingDto.type == "accommodation")
            {
                var stayed = requests.Any(
                    r => r.AccommodationId == parsedAccommodationId
                    && r.State == RequestState.ACCEPTED 
                    && r.EndDate <= DateTime.UtcNow
                );

                if (!stayed)
                {
                    return BadRequest("Guest has not stayed at this accommodation.");
                }

                rating.AccommodationId = parsedAccommodationId;
                rating.HostId = Guid.Empty;
            }
            else if (ratingDto.type == "host")
            {
                var stayed = requests.Any(
                    r => r.Accommodation.OwnerId == parsedHostId
                    && r.State != RequestState.USER_REJECT
                );

                if (!stayed)
                {
                    return BadRequest("Guest has not stayed with this host.");
                }

                rating.AccommodationId = Guid.Empty;
                rating.HostId = parsedHostId;
            }
            else
            {
                return BadRequest("Invalid rating type.");
            }

            var createdRating = _ratingService.AddRating(rating);
            return Ok(createdRating);
        }

        [HttpPut("{ratingId}")]
        public IActionResult UpdateRating(Guid ratingId, [FromBody] int newEvaluation)
        {
            if (newEvaluation < 1 || newEvaluation > 5) {
                return BadRequest("Evaluation must be between 1 and 5.");
            }

            _ratingService.UpdateRating(ratingId, newEvaluation);
            return Ok();
        }

        [HttpDelete("{ratingId}")]
        public IActionResult DeleteRating(Guid ratingId)
        {
            _ratingService.DeleteRating(ratingId);
            return Ok();
        }

        [HttpGet("accommodation/{accommodationId}/average")]
        public IActionResult GetAverageRatingForAccommodation(Guid accommodationId)
        {
            var averageRating = _ratingService.GetAverageRatingForAccommodation(accommodationId);
            return Ok(averageRating);
        }

        [HttpGet("host/{hostId}/average")]
        public IActionResult GetAverageRatingForHost(Guid hostId)
        {
            var averageRating = _ratingService.GetAverageRatingForHost(hostId);
            return Ok(averageRating);
        }

        [HttpGet]
        public IActionResult GetRatings(Guid guestId)
        {
            var accommodations = _accommodationService.GetAll();

            var accommodationRatings = new List<AccommodationWithRatingsDto>();
            var hostRatings = new List<HostWithRatingsDto>();

            foreach (var acc in accommodations)
            {
                var accRatings = _ratingService.GetRatingsForAccommodation(acc.Id);
                var accAverage = _ratingService.GetAverageRatingForAccommodation(acc.Id).ToString("F2");
                accommodationRatings.Add(new AccommodationWithRatingsDto
                {
                    AccommodationId = acc.Id.ToString(),
                    AccommodationName = acc.Name,
                    Average = accAverage,
                    Ratings = accRatings
                });

                if (!hostRatings.Any(h => h.HostId == acc.OwnerId.ToString()))
                {
                    var hRatings = _ratingService.GetRatingsForHost(acc.OwnerId);
                    var hAverage = _ratingService.GetAverageRatingForHost(acc.OwnerId).ToString("F2");
                    hostRatings.Add(new HostWithRatingsDto
                    {
                        HostId = acc.OwnerId.ToString(),
                        HostUsername = _userService.Get(acc.OwnerId).Username,
                        Average = hAverage,
                        Ratings = hRatings
                    });
                }
            }

            var result = new AccAndHostsWithRatingsDto
            {
                Accommodations = accommodationRatings,
                Hosts = hostRatings
            };
            return Ok(result);
        }

        [HttpGet("canRate/{guestId}")]
        public IActionResult CanRate(Guid guestId)
        {
            var requests = _requestService.GetAllByGuestId(guestId);
            var result = new CanRateDto
            {
                Accommodations = new List<string>(),
                Hosts = new List<string>()
            };

            foreach (var r in requests)
            {
                if (r.GuestId != guestId) continue;

                if (r.State == RequestState.ACCEPTED && r.EndDate <= DateTime.UtcNow)
                {
                    if (!result.Accommodations.Contains(r.AccommodationId.ToString()))
                    {
                        result.Accommodations.Add(r.AccommodationId.ToString());
                    }
                }

                if (r.State != RequestState.USER_REJECT)
                {
                    if (!result.Hosts.Contains(r.Accommodation.OwnerId.ToString()))
                    {
                        result.Hosts.Add(r.Accommodation.OwnerId.ToString());
                    }
                }
            }

            return Ok(result);
        }
    }
}
