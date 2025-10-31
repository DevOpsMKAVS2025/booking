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
            CreateMap<Request, RequestDto>()
                .ForMember(dest => dest.Accommodation, opt => opt.MapFrom(src => src.Accommodation.Name));
            CreateMap<RequestDto, Request>()
                .ForMember(dest => dest.Accommodation, opt => opt.Ignore());
            CreateMap<RequestWithCancelCountDto, Request>().ReverseMap();
        }
    }
}
