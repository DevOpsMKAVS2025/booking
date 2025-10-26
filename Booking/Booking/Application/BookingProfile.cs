using AutoMapper;
using Booking.Application.Dtos;
using Booking.Domain.Entities;

namespace Booking.Application
{
    public class BookingProfile: Profile
    {
        public BookingProfile() 
        {
            CreateMap<AccommodationDto, Accommodation>().ReverseMap();
            CreateMap<PriceDto, Price>().ReverseMap();
            CreateMap<AvailabilityDto, Availability>().ReverseMap();
            CreateMap<RequestDto, Request>().ReverseMap();
            CreateMap<RequestWithCancelCountDto, Request>().ReverseMap();
        }
    }
}
