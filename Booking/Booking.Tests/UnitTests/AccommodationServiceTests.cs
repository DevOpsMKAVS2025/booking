using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.UseCases;
using Booking.BuildingBlocks.Core;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Booking.Tests.Application.UseCases
{
    public class AccommodationServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAccommodationRepository> _repositoryMock;
        private readonly AccommodationService _service;

        public AccommodationServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IAccommodationRepository>();
            var options = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(options);
            _service = new AccommodationService(_mapperMock.Object, _repositoryMock.Object, cache);
        }
        private static void SetId(Entity entity, Guid id)
        {
            typeof(Entity).GetProperty("Id")!
                .SetValue(entity, id);
        }

        [Fact]
        public void GetPaged_ShouldReturnPagedResult()
        {
            var paged = new PagedResult<Accommodation>(new List<Accommodation> { new Accommodation() }, 1);
            var dtoPaged = new PagedResult<AccommodationDto>(new List<AccommodationDto> { new AccommodationDto() }, 1);

            _repositoryMock.Setup(r => r.GetPaged(1, 10)).Returns(paged);
            _mapperMock.Setup(m => m.Map<PagedResult<AccommodationDto>>(paged)).Returns(dtoPaged);

            var result = _service.GetPaged(1, 10);

            result.IsSuccess.Should().BeTrue();
            result.Value.Results.Should().HaveCount(1);
        }

        [Fact]
        public void Get_ShouldReturnAccommodation_WhenExists()
        {
            var id = Guid.NewGuid();
            var acc = new Accommodation { Name = "Hotel" };
            SetId(acc, id);
            var dto = new AccommodationDto { Id = id, Name = "Hotel" };

            _repositoryMock.Setup(r => r.Get(id)).Returns(acc);
            _mapperMock.Setup(m => m.Map<AccommodationDto>(acc)).Returns(dto);

            var result = _service.Get(id);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public void Get_ShouldFail_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.Get(It.IsAny<Guid>())).Throws(new KeyNotFoundException("Not found"));

            var result = _service.Get(Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldReturnAccommodation_WhenValid()
        {
            var dto = new AccommodationDto();
            var acc = new Accommodation();

            _mapperMock.Setup(m => m.Map<Accommodation>(dto)).Returns(acc);
            _repositoryMock.Setup(r => r.Create(acc)).Returns(acc);
            _mapperMock.Setup(m => m.Map<AccommodationDto>(acc)).Returns(dto);

            var result = _service.Create(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.Create(It.IsAny<Accommodation>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.Create(new AccommodationDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void Update_ShouldReturnUpdatedAccommodation()
        {
            var dto = new AccommodationDto();
            var acc = new Accommodation();

            _mapperMock.Setup(m => m.Map<Accommodation>(dto)).Returns(acc);
            _repositoryMock.Setup(r => r.Update(acc)).Returns(acc);
            _mapperMock.Setup(m => m.Map<AccommodationDto>(acc)).Returns(dto);

            var result = _service.Update(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Update_ShouldFail_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.Update(It.IsAny<Accommodation>()))
                .Throws(new KeyNotFoundException("Not found"));

            var result = _service.Update(new AccommodationDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void Delete_ShouldReturnOk_WhenDeleted()
        {
            var result = _service.Delete(Guid.NewGuid());

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Delete_ShouldFail_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.Delete(It.IsAny<Guid>()))
                .Throws(new KeyNotFoundException("Not found"));

            var result = _service.Delete(Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void CreatePrice_ShouldReturnPrice_WhenValid()
        {
            var dto = new PriceDto();
            var price = new Price();

            _mapperMock.Setup(m => m.Map<Price>(dto)).Returns(price);
            _repositoryMock.Setup(r => r.CreatePrice(price)).Returns(price);
            _mapperMock.Setup(m => m.Map<PriceDto>(price)).Returns(dto);

            var result = _service.CreatePrice(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void CreatePrice_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.CreatePrice(It.IsAny<Price>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.CreatePrice(new PriceDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void UpdatePrice_ShouldReturnUpdatedPrice()
        {
            var dto = new PriceDto();
            var price = new Price();

            _mapperMock.Setup(m => m.Map<Price>(dto)).Returns(price);
            _repositoryMock.Setup(r => r.UpdatePrice(price)).Returns(price);
            _mapperMock.Setup(m => m.Map<PriceDto>(price)).Returns(dto);

            var result = _service.UpdatePrice(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void UpdatePrice_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.UpdatePrice(It.IsAny<Price>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.UpdatePrice(new PriceDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void GetPrice_ShouldReturnPrice()
        {
            var accId = Guid.NewGuid();
            var priceId = Guid.NewGuid();
            var price = new Price();
            var dto = new PriceDto();

            _repositoryMock.Setup(r => r.GetPrice(accId, priceId)).Returns(price);
            _mapperMock.Setup(m => m.Map<PriceDto>(price)).Returns(dto);

            var result = _service.GetPrice(accId, priceId);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void GetPrice_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.GetPrice(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.GetPrice(Guid.NewGuid(), Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void CreateAvailability_ShouldReturnAvailability()
        {
            var dto = new AvailabilityDto();
            var availability = new Availability();

            _mapperMock.Setup(m => m.Map<Availability>(dto)).Returns(availability);
            _repositoryMock.Setup(r => r.CreateAvailability(availability)).Returns(availability);
            _mapperMock.Setup(m => m.Map<AvailabilityDto>(availability)).Returns(dto);

            var result = _service.CreateAvailability(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void CreateAvailability_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.CreateAvailability(It.IsAny<Availability>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.CreateAvailability(new AvailabilityDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void UpdateAvailability_ShouldReturnUpdatedAvailability()
        {
            var dto = new AvailabilityDto();
            var availability = new Availability();

            _mapperMock.Setup(m => m.Map<Availability>(dto)).Returns(availability);
            _repositoryMock.Setup(r => r.UpdateAvailability(availability)).Returns(availability);
            _mapperMock.Setup(m => m.Map<AvailabilityDto>(availability)).Returns(dto);

            var result = _service.UpdateAvailability(dto);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void UpdateAvailability_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.UpdateAvailability(It.IsAny<Availability>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.UpdateAvailability(new AvailabilityDto());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void GetAvailability_ShouldReturnAvailability()
        {
            var accId = Guid.NewGuid();
            var availId = Guid.NewGuid();
            var avail = new Availability();
            var dto = new AvailabilityDto();

            _repositoryMock.Setup(r => r.GetAvailability(accId, availId)).Returns(avail);
            _mapperMock.Setup(m => m.Map<AvailabilityDto>(avail)).Returns(dto);

            var result = _service.GetAvailability(accId, availId);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void GetAvailability_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.GetAvailability(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.GetAvailability(Guid.NewGuid(), Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void GetByOwnerId_ShouldReturnAccommodations()
        {
            var ownerId = Guid.NewGuid();
            var accommodations = new List<Accommodation> { new Accommodation() };
            var dtos = new List<AccommodationDto> { new AccommodationDto() };

            _repositoryMock.Setup(r => r.GetByOwnerId(ownerId)).Returns(accommodations);
            _mapperMock.Setup(m => m.Map<List<AccommodationDto>>(accommodations)).Returns(dtos);

            var result = _service.GetByOwnerId(ownerId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public void GetByOwnerId_ShouldFail_WhenInvalid()
        {
            _repositoryMock.Setup(r => r.GetByOwnerId(It.IsAny<Guid>()))
                .Throws(new ArgumentException("Invalid"));

            var result = _service.GetByOwnerId(Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void ToggleAutoReservation_ShouldToggleFlag()
        {
            var id = Guid.NewGuid();
            var acc = new Accommodation { IsAutoReservation = false };
            var dto = new AccommodationDto { Id = id, IsAutoReservation = true };

            SetId(acc, id);

            _repositoryMock.Setup(r => r.Get(id)).Returns(acc);
            _repositoryMock.Setup(r => r.Update(It.IsAny<Accommodation>())).Returns(acc);
            _mapperMock.Setup(m => m.Map<AccommodationDto>(acc)).Returns(dto);

            var result = _service.ToggleAutoReservation(id);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ToggleAutoReservation_ShouldFail_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.Get(It.IsAny<Guid>())).Returns((Accommodation)null);

            var result = _service.ToggleAutoReservation(Guid.NewGuid());

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public void GetAll_ShouldReturnList()
        {
            _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Accommodation> { new Accommodation() });

            var result = _service.GetAll();

            result.Should().HaveCount(1);
        }
    }
}
