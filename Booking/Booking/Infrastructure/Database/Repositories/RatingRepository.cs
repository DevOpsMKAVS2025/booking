using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Database.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        protected readonly BookingDbContext _bookingDbContext;
        protected readonly AuthDBContext _authDbContext;

        public RatingRepository(AuthDBContext authDbContext, BookingDbContext bookingDbContext)
        {
            _authDbContext = authDbContext;
            _bookingDbContext = bookingDbContext;
        }

        public List<Rating> GetRatingsForAccommodation(Guid accommodationId)
        {
            return _bookingDbContext.Ratings
                .Where(r => r.AccommodationId == accommodationId && !r.IsDeleted)
                .ToList();
        }

        public List<Rating> GetRatingsForHost(Guid hostId)
        {
            return _bookingDbContext.Ratings
                .Where(r => r.HostId == hostId && !r.IsDeleted)
                .ToList();
        }

        public Rating AddRating(Rating rating)
        {
            _bookingDbContext.Ratings.Add(rating);
            _bookingDbContext.SaveChanges();
            return rating;
        }

        public void DeleteRating(Guid ratingId)
        {
            Rating rating = _bookingDbContext.Ratings.Find(ratingId);
            if (rating == null || rating.IsDeleted)
            {
                throw new KeyNotFoundException("Rating not found: " + ratingId);
            }
            rating.IsDeleted = true;
            _bookingDbContext.SaveChanges();
        }

        public void UpdateRating(Guid ratingId, int newEvaluation)
        {
            var existingRating = _bookingDbContext.Ratings.Find(ratingId);
            if (existingRating == null || existingRating.IsDeleted)
            {
                throw new KeyNotFoundException("Rating not found: " + ratingId);
            }
            existingRating.Evaluation = newEvaluation;
            existingRating.GivenAt = DateTime.UtcNow;
            _bookingDbContext.SaveChanges();
        }
    }
}
