namespace Booking.Domain.Entities.RepositoryInterfaces
{
    public interface IRatingRepository
    {
        List<Rating> GetRatingsForAccommodation(Guid accommodationId);
        List<Rating> GetRatingsForHost(Guid hostId);
        Rating AddRating(Rating rating);
        void DeleteRating(Guid ratingId);
        void UpdateRating(Guid ratingId, int newEvaluation);
    }
}
