using Booking.Domain.Entities;
using Booking.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Tests.IntegrationTests
{
    public class RatingServiceTests : IntegrationTestBase
    {
        [Fact]
        public async Task TestAverageRatingForAccommodation()
        {
            using var bookingdBContext = GetDbContext<BookingDbContext>();

            Guid accGuid = Guid.NewGuid();
            var rating1 = new Rating { AccommodationId = accGuid, Evaluation = 3, IsDeleted = false };
            var rating2 = new Rating { AccommodationId = accGuid, Evaluation = 4, IsDeleted = false };

            bookingdBContext.Ratings.Add(rating1);
            bookingdBContext.Ratings.Add(rating2);

            bookingdBContext.SaveChanges();

            var response = await _httpClient.GetAsync($"/api/rating/accommodation/{accGuid}/average");
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal("3.5", responseString);
        }
    }
}
