using System.Runtime.CompilerServices;
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
        public Result<PriceDto> GetPrice(Guid accommodationId, Guid priceId)
        {
            try
            {
                var result = _repository.GetPrice(accommodationId, priceId);
                return Result.Ok(_mapper.Map<PriceDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public Result<AvailabilityDto> CreateAvailability(AvailabilityDto availabilityDto)
        {
            try
            {
                var result = _repository.CreateAvailability(_mapper.Map<Availability>(availabilityDto));
                return Result.Ok(_mapper.Map<AvailabilityDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public Result<AvailabilityDto> UpdateAvailability(AvailabilityDto availabilityDto)
        {
            try
            {
                var result = _repository.UpdateAvailability(_mapper.Map<Availability>(availabilityDto));
                return Result.Ok(_mapper.Map<AvailabilityDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public Result<AvailabilityDto> GetAvailability(Guid accommodationId, Guid availabilityId)
        {
            try
            {
                var result = _repository.GetAvailability(accommodationId, availabilityId);
                return Result.Ok(_mapper.Map<AvailabilityDto>(result));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        public PagedResult<AccommodationAndPriceDto> GetAccomodationByFilters(string? location, int guestNumber, DateTime from, DateTime to)
        {
            var accommodations = _repository.GetPaged(0, 0);

            var filteredAccommodations = accommodations.Results.AsQueryable();

            List<AccommodationAndPriceDto> accommodationAndPriceDtos = new List<AccommodationAndPriceDto>();

            if (!string.IsNullOrWhiteSpace(location))
            {
                filteredAccommodations = filteredAccommodations
                    .Where(x => x.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
            }

            filteredAccommodations = filteredAccommodations
                .Where(x => x.MinGuestNumber <= guestNumber && x.MaxGuestNumber >= guestNumber);
            
            List<Accommodation> availableAccommodations = new();
            
            foreach (var acc in filteredAccommodations)
            {
                if (IsAvailable(acc, from, to))
                    availableAccommodations.Add(acc);
            }
            

            foreach (var acc in availableAccommodations)
            {
                var totalPrice = CalculateTotalPrice(acc, from, to, guestNumber);
                var pricePer = CalculatePricePerNightOrPerson(totalPrice, acc, (to.Date - from.Date).Days, guestNumber);
                accommodationAndPriceDtos.Add(new AccommodationAndPriceDto(
                    acc.Id, 
                    acc.Name, 
                    acc.Location, 
                    acc.Conveniences, 
                    acc.Photos, 
                    acc.MinGuestNumber, 
                    acc.MaxGuestNumber, 
                    totalPrice, 
                    pricePer));
            }

            return new PagedResult<AccommodationAndPriceDto>(accommodationAndPriceDtos, accommodationAndPriceDtos.Count);
        }

        public Result<List<AccommodationDto>> GetByOwnerId(Guid id)
        {
            try
            {
                List<Accommodation> accommodations = _repository.GetByOwnerId(id);
                return Result.Ok(_mapper.Map<List<AccommodationDto>>(accommodations));
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }

        private decimal CalculateTotalPrice( Accommodation accommodation, DateTime fromDate, DateTime toDate, int guestCount)
        {
            decimal total = 0m;

            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                var priceForDay = accommodation.Prices
                    ?.FirstOrDefault(p => p.Duration.From <= date && p.Duration.To >= date);

                decimal dayPrice;

                if (priceForDay != null)
                {
                    dayPrice = priceForDay.Amount;

                    if (priceForDay.PriceType == PriceType.PER_GUEST)
                        dayPrice *= guestCount;
                }
                else
                {
                    dayPrice = accommodation.GlobalPrice;
                }

                total += dayPrice;
            }

            return total;
        }
        private decimal CalculatePricePerNightOrPerson(decimal price, Accommodation accommodation, int numberOfDays, int guestCount)
        {
            if (accommodation.Prices[0].PriceType == PriceType.PER_GUEST)
                return price/guestCount/numberOfDays;
            else
                return price/numberOfDays;
        }
        private bool IsAvailable(Accommodation accommodation, DateTime fromDate, DateTime toDate)
        {
            if (accommodation.Availability == null || accommodation.Availability.Count == 0)
                return false;

            var sorted = accommodation.Availability
                .OrderBy(a => a.Duration.From)
                .ToList();

            var merged = new List<DateRange>();
            var current = sorted[0].Duration;
            if(sorted.Count == 1)
            {
                merged.Add(current);
                return merged.Any(range =>
                    range.From.Date <= fromDate.Date && range.To.Date >= toDate.Date
                );
            }
            

            for (int i = 1; i < sorted.Count; i++)
            {
                var next = sorted[i].Duration;
                if (next.From <= current.To.AddDays(1))
                {
                    current.To = next.To > current.To ? next.To : current.To;
                }
                else
                {
                    merged.Add(current);
                    current = next;
                }
            }
            merged.Add(current);

            return merged.Any(range =>
                range.From <= fromDate && range.To >= toDate
            );
        }
    }
}
