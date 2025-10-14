using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.BuildingBlocks.Core;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using FluentResults;

namespace Booking.Application.UseCases
{
    public class AccommodationService : BaseService<AccommodationDto, Accommodation>, IAccommodationService
    {
        protected readonly IMapper _mapper;
        protected readonly IAccommodationRepository _repository;

        public AccommodationService(IMapper mapper, IAccommodationRepository accommodationRepository) : base(mapper)
        {
            _mapper = mapper;
            _repository = accommodationRepository;
        }

        public Result<PagedResult<AccommodationDto>> GetPaged(int page, int pageSize)
        {
            var result = _repository.GetPaged(page, pageSize);
            return MapToDto(result);
        }

        public Result<AccommodationDto> Get(Guid id)
        {
            try
            {
                var result = _repository.Get(id);
                return MapToDto(result);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<AccommodationDto> Create(AccommodationDto entity)
        {
            try
            {
                var result = _repository.Create(MapToDomain(entity));
                return MapToDto(result);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public Result<AccommodationDto> Update(AccommodationDto entity)
        {
            try
            {
                var result = _repository.Update(MapToDomain(entity));
                return MapToDto(result);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public Result Delete(Guid id)
        {
            try
            {
                _repository.Delete(id);
                return Result.Ok();
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<PriceDto> CreatePrice(PriceDto priceDto)
        {
            try
            {
                var result = _repository.CreatePrice(_mapper.Map<Price>(priceDto));
                return Result.Ok(_mapper.Map<PriceDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }
        public Result<PriceDto> UpdatePrice(PriceDto priceDto)
        {
            try
            {
                var result = _repository.UpdatePrice(_mapper.Map<Price>(priceDto));
                return Result.Ok(_mapper.Map<PriceDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }
        public Result<PriceDto> GetPrice(Guid AccommodationId, Guid priceId)
        {
            try
            {
                var result = _repository.GetPrice(AccommodationId, priceId);
                return Result.Ok(_mapper.Map<PriceDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }
    }
}
