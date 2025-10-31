using Booking.Application.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Tests.UnitTests
{
    public class RatingServiceTests
    {
        private readonly Mock<IRatingRepository> _mockRatingRepository;
        private readonly Mock<IRequestRepository> _mockRequestRepository;

        private readonly RatingService _service;

        public RatingServiceTests()
        {
            _mockRatingRepository = new Mock<IRatingRepository>();
            _mockRequestRepository = new Mock<IRequestRepository>();

            _service = new RatingService(_mockRatingRepository.Object, _mockRequestRepository.Object);
        }

        [Fact]
        public void GetAverageRatingForAccommodation_NonexistentId_ThrowsException()
        {
            var nonExistingId = Guid.NewGuid();

            _mockRatingRepository
                .Setup(r => r.GetRatingsForAccommodation(nonExistingId))
                .Throws(new KeyNotFoundException("Accommodation not found"));

            Assert.Throws<KeyNotFoundException>(() =>
                _service.GetAverageRatingForAccommodation(nonExistingId));
        }

        [Fact]
        public void GetAverageRatingForAccommodation_NoRatings_ReturnsZero()
        {
            var accommodationId = Guid.NewGuid();

            _mockRatingRepository
                .Setup(r => r.GetRatingsForAccommodation(accommodationId))
                .Returns(new List<Rating>());

            var result = _service.GetAverageRatingForAccommodation(accommodationId);

            Assert.Equal(0.0, result);
        }

        [Fact]
        public void GetAverageRatingForAccommodation_WithRatings_ReturnsAverage()
        {
            var accommodationId = Guid.NewGuid();

            var ratings = new List<Rating>
            {
                new Rating { Evaluation = 4 },
                new Rating { Evaluation = 2 },
                new Rating { Evaluation = 3 }
            };

            _mockRatingRepository
                .Setup(r => r.GetRatingsForAccommodation(accommodationId))
                .Returns(ratings);

            var result = _service.GetAverageRatingForAccommodation(accommodationId);

            Assert.Equal(3.0, result);
        }
    }
}
