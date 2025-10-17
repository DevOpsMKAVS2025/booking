using Booking.Domain.Entities;

namespace Booking.Application.Dtos
{
    public class AccommodationAndPriceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ConvenieceType> Conveniences { get; set; }
        public List<string> Photos { get; set; }
        public int MinGuestNumber { get; set; }
        public int MaxGuestNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerPersonOrNight { get; set; }

        public AccommodationAndPriceDto(Guid id, string name, string location, List<ConvenieceType> conveniences, List<string> photos, int minGuestNumber, int maxGuestNumber, decimal totalPrice, decimal pricePerPersonOrNight)
        {
            Id = id;
            Name = name;
            Location = location;
            Conveniences = conveniences;
            Photos = photos;
            MinGuestNumber = minGuestNumber;
            MaxGuestNumber = maxGuestNumber;
            TotalPrice = totalPrice;
            PricePerPersonOrNight = pricePerPersonOrNight;
        }
    }
}
