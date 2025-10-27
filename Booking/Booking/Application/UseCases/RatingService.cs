using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;

namespace Booking.Application.UseCases
{
    public class RatingService : IRatingService
    {
        protected readonly IRatingRepository _repository;
        protected readonly IRequestRepository _requestRepository;

        public RatingService(IRatingRepository ratingRepository, IRequestRepository requestRepository)
        {
            _repository = ratingRepository;
            _requestRepository = requestRepository;
        }

        public List<Rating> GetRatingsForAccommodation(Guid accommodationId)
        {
            return _repository.GetRatingsForAccommodation(accommodationId);
        }

        public List<Rating> GetRatingsForHost(Guid hostId)
        {
            return _repository.GetRatingsForHost(hostId);
        }

        public Rating AddRating(Rating rating)
        {
            return _repository.AddRating(rating);
        }

        public void DeleteRating(Guid ratingId)
        {
            _repository.DeleteRating(ratingId);
        }

        public void UpdateRating(Guid ratingId, int newEvaluation)
        {
            _repository.UpdateRating(ratingId, newEvaluation);
        }

        public double GetAverageRatingForAccommodation(Guid accommodationId)
        {
            var ratings = _repository.GetRatingsForAccommodation(accommodationId);
            if (ratings.Count == 0) return 0.0;
            return ratings.Average(r => r.Evaluation);
        }

        public double GetAverageRatingForHost(Guid hostId)
        {
            var ratings = _repository.GetRatingsForHost(hostId);
            if (ratings.Count == 0) return 0.0;
            return ratings.Average(r => r.Evaluation);
        }
    }
}
