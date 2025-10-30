using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using FluentResults;

namespace Booking.Application.Interfaces
{
    public interface IAccommodationService
    {
        public Result<PagedResult<AccommodationDto>> GetPaged(int page, int pageSize);
        public Task<Result<AccommodationDto>>Get(Guid id);
        public Result<AccommodationDto> Create(AccommodationDto entity);
        public Result<AccommodationDto> Update(AccommodationDto entity);
        public Result Delete(Guid id);
        public Result<PriceDto> CreatePrice(PriceDto priceDto);
        public Result<PriceDto> UpdatePrice(PriceDto priceDto);
        public Result<PriceDto> GetPrice(Guid accommodationId, Guid priceId);
        public Result<AvailabilityDto> CreateAvailability(AvailabilityDto availabilityDto);
        public Result<AvailabilityDto> UpdateAvailability(AvailabilityDto availabilityDto);
        public Result<AvailabilityDto> GetAvailability(Guid accommodationId, Guid availabilityId);
        public PagedResult<AccommodationAndPriceDto> GetAccomodationByFilters(string? location, int guestNumber,DateTime from, DateTime to);
        public Result<AccommodationDto> ToggleAutoReservation(Guid accommodationId);
        public Result<List<AccommodationDto>> GetByOwnerId(Guid id);
        public List<Accommodation> GetAll();
        Task DeleteAccommodationsForOwner(Guid ownerId);
    }
}
