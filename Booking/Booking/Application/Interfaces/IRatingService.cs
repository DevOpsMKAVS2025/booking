using Booking.Domain.Entities;

namespace Booking.Application.Interfaces
{
    public interface IRatingService
    {
        List<Rating> GetRatingsForAccommodation(Guid accommodationId);
        List<Rating> GetRatingsForHost(Guid hostId);
        Rating AddRating(Rating rating);
        void DeleteRating(Guid ratingId);
        void UpdateRating(Guid ratingId, int newEvaluation);
        double GetAverageRatingForAccommodation(Guid accommodationId);
        double GetAverageRatingForHost(Guid hostId);
    }
}
