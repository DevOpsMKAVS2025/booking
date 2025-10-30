using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Booking.Infrastructure.Database;
using Booking.Infrastructure.Database.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Booking.Tests.IntegrationTests
{
    public class AccommodationServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IMapper _mapper;

        public AccommodationServiceIntegrationTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Accommodation, AccommodationDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Create_And_Get_ShouldReturnSameAccommodation()
        {
            // Arrange
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAccommodationRepository>();
            var options = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(options);
            var service = new AccommodationService(_mapper, repository, cache);

            var dto = new AccommodationDto
            {
                Id = Guid.NewGuid(),
                Name = "Integration Test Hotel",
                Location = "Belgrade",
                MinGuestNumber = 1,
                MaxGuestNumber = 4
            };

            // Act
            var createResult = service.Create(dto);
            var getResult = service.Get(dto.Id.Value);

            // Assert
            createResult.IsSuccess.Should().BeTrue();
            getResult.IsSuccess.Should().BeTrue();
            getResult.Value.Name.Should().Be("Integration Test Hotel");
            getResult.Value.Location.Should().Be("Belgrade");
        }

        [Fact]
        public async Task Update_ShouldModifyAccommodation()
        {
            Guid id;

            // Arrange - Create
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IAccommodationRepository>();
                var options = Options.Create(new MemoryDistributedCacheOptions());
                IDistributedCache cache = new MemoryDistributedCache(options);
                var service = new AccommodationService(_mapper, repository, cache);

                var dto = new AccommodationDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Old Hotel",
                    Location = "Novi Sad",
                    MinGuestNumber = 1,
                    MaxGuestNumber = 3
                };

                service.Create(dto);
                id = dto.Id.Value;
            }

            // Act - Update
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IAccommodationRepository>();
                var options = Options.Create(new MemoryDistributedCacheOptions());
                IDistributedCache cache = new MemoryDistributedCache(options);
                var service = new AccommodationService(_mapper, repository, cache);

                var dto = new AccommodationDto
                {
                    Id = id,
                    Name = "Updated Hotel",
                    Location = "Novi Sad",
                    MinGuestNumber = 1,
                    MaxGuestNumber = 3
                };

                var updateResult = service.Update(dto);
                var getResult = service.Get(id);

                // Assert
                updateResult.IsSuccess.Should().BeTrue();
                getResult.IsSuccess.Should().BeTrue();
                getResult.Value.Name.Should().Be("Updated Hotel");
            }
        }

        [Fact]
        public async Task Delete_ShouldRemoveAccommodation()
        {
            // Arrange
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAccommodationRepository>();
            var options = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(options);
            var service = new AccommodationService(_mapper, repository, cache);

            var dto = new AccommodationDto
            {
                Id = Guid.NewGuid(),
                Name = "To Be Deleted",
                Location = "Niš",
                MinGuestNumber = 2,
                MaxGuestNumber = 5
            };

            service.Create(dto);

            // Act
            var deleteResult = service.Delete(dto.Id.Value);
            var getResult = service.Get(dto.Id.Value);

            // Assert
            deleteResult.IsSuccess.Should().BeTrue();
            getResult.IsFailed.Should().BeTrue();
            getResult.Errors.First().Message.Should().ContainEquivalentOf("not found");
        }
    }
}
