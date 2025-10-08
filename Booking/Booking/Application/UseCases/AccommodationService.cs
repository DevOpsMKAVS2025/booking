using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using FluentResults;

namespace Booking.Application.UseCases
{
    public class AccommodationService : CrudService<AccommodationDto, Accommodation>, IAccommodationService
    {
        protected readonly IMapper _mapper;
        protected readonly ICrudRepository<Accommodation> _repository;

        public AccommodationService(ICrudRepository<Accommodation> crudRepository, IMapper mapper) : base(crudRepository, mapper)
        {
            _mapper = mapper;
            _repository = crudRepository;
        }
    }
}
