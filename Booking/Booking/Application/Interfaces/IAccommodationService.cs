using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using FluentResults;

namespace Booking.Application.Interfaces
{
    public interface IAccommodationService
    {
        public Result<PagedResult<AccommodationDto>> GetPaged(int page, int pageSize);
        public Result<AccommodationDto> Get(Guid id);
        public Result<AccommodationDto> Create(AccommodationDto entity);
        public Result<AccommodationDto> Update(AccommodationDto entity);
        public Result Delete(Guid id);
        public Result<PriceDto> CreatePrice(PriceDto priceDto);
        public Result<PriceDto> UpdatePrice(PriceDto priceDto);
        public Result<PriceDto> GetPrice(Guid AccommodationId, Guid priceId);
    }
}
