using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.UseCases;
using Booking.BuildingBlocks.Core;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Booking.Tests.UnitTests
{
    public class RequestServiceTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IDatabase> _databaseMock = new();
        private readonly Mock<IRequestRepository> _requestRepositoryMock = new();
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new();

        private readonly RequestService _service;

        public RequestServiceTests()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Redis:ConnectionString"]).Returns("localhost:6379");

            _service = new RequestService(
                _mapperMock.Object,
                configurationMock.Object,
                _requestRepositoryMock.Object,
                _accommodationRepositoryMock.Object
            );
        }

        private static T SetId<T>(T entity, Guid id)
        {
            var property = typeof(T).GetProperty(nameof(Entity.Id),
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);

            property!.SetValue(entity, id);
            return entity;
        }

        [Fact]
        public async Task CreateRequest_ShouldReturnFail_WhenAccommodationNotFound()
        {
            var dto = new RequestDto { AccommodationId = Guid.NewGuid() };
            _accommodationRepositoryMock.Setup(r => r.Get(dto.AccommodationId))
                .Returns((Accommodation)null);

            var result = await _service.CreateRequest(dto);

            Assert.True(result.IsFailed);
            Assert.Equal("Accommodation not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateRequest_ShouldReturnFail_WhenGuestsOutOfRange()
        {
            var dto = new RequestDto
            {
                AccommodationId = Guid.NewGuid(),
                GuestNum = 10
            };

            var accommodation = new Accommodation
            {
                MinGuestNumber = 1,
                MaxGuestNumber = 5
            };

            _accommodationRepositoryMock.Setup(r => r.Get(dto.AccommodationId))
                .Returns(accommodation);

            var result = await _service.CreateRequest(dto);

            Assert.True(result.IsFailed);
            Assert.Contains("Number of guests", result.Errors.First().Message);
        }

        [Fact]
        public async Task ApproveRequest_ShouldReturnFail_WhenRequestNotPending()
        {
            var requestId = Guid.NewGuid();
            var request = SetId(new Request { State = RequestState.ACCEPTED }, requestId);

            _requestRepositoryMock.Setup(r => r.GetById(It.Is<Guid>(id => id == requestId)))
                                  .ReturnsAsync(request);

            var result = await _service.ApproveRequest(requestId);

            Assert.True(result.IsFailed);
            Assert.Equal("Request not found or cannot be approved", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetAcceptedByAccommodationId_ShouldReturnFromCache_WhenCacheExists()
        {
            var accommodationId = Guid.NewGuid();
            var cacheKey = $"accepted_requests_{accommodationId}";
            var cachedRequests = new List<RequestDto>
            {
                new RequestDto { Id = Guid.NewGuid() }
            };
            var serializedCachedRequests = JsonSerializer.Serialize(cachedRequests);

            _databaseMock
                    .Setup(db => db.StringGetAsync(cacheKey, It.IsAny<CommandFlags>()))
                    .ReturnsAsync((RedisValue)serializedCachedRequests);
            
            var result = await _service.GetAcceptedByAccommodationId(accommodationId);

            Assert.True(result.IsSuccess);
            _requestRepositoryMock.Verify(r => r.GetAcceptedByAccommodationId(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetAcceptedByAccommodationId_ShouldQueryRepository_WhenCacheEmpty()
        {
            var accommodationId = Guid.NewGuid();
            var request = SetId(new Request { State = RequestState.ACCEPTED }, Guid.NewGuid());
            var requests = new List<Request> { request };
            var cacheKey = $"accepted_requests_{accommodationId}";

            _databaseMock
                .Setup(db => db.StringGetAsync(cacheKey, It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            _requestRepositoryMock
                .Setup(r => r.GetAcceptedByAccommodationId(accommodationId))
                .ReturnsAsync(requests);

            var dtos = requests.Select(r => new RequestDto { Id = r.Id }).ToList();
            _mapperMock
                .Setup(m => m.Map<IEnumerable<RequestDto>>(requests))
                .Returns(dtos);

            _databaseMock
                .Setup(db => db.StringSetAsync(
                    cacheKey,
                    "anystring",
                    TimeSpan.FromSeconds(5),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            var result = await _service.GetAcceptedByAccommodationId(accommodationId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task RejectReservation_ShouldFail_WhenRequestNotFound()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _requestRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
                                  .ReturnsAsync((Request)null);

            // Act
            var result = await _service.RejectReservation(requestId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Reservation not found or cannot be rejected", result.Errors.First().Message);
        }

        [Fact]
        public async Task RejectReservation_ShouldFail_WhenStateIsPending()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = SetId(new Request { State = RequestState.PENDING }, requestId);

            _requestRepositoryMock.Setup(r => r.GetById(requestId))
                                  .ReturnsAsync(request);

            // Act
            var result = await _service.RejectReservation(requestId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Reservation not found or cannot be rejected", result.Errors.First().Message);
        }

        [Fact]
        public async Task RejectReservation_ShouldSucceed_WhenRequestIsAccepted()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var accommodationId = Guid.NewGuid();

            var request = SetId(new Request
            {
                State = RequestState.ACCEPTED,
                AccommodationId = accommodationId
            }, requestId);

            _requestRepositoryMock.Setup(r => r.GetById(requestId))
                                  .ReturnsAsync(request);

            _requestRepositoryMock.Setup(r => r.Update(request))
                                  .Returns(Task.CompletedTask);

            _requestRepositoryMock.Setup(r => r.SaveChanges())
                                  .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<RequestDto>(request))
               .Returns(new RequestDto { Id = requestId });

            var cacheKey = $"accepted_requests_{accommodationId}";
            _databaseMock.Setup(db => db.KeyDeleteAsync(cacheKey, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _service.RejectReservation(requestId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(RequestState.USER_REJECT, request.State);
            _requestRepositoryMock.Verify(r => r.Update(request), Times.Once);
        }
    }
}