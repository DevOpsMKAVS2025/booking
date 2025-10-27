using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.UseCases;
using Booking.BuildingBlocks.Core;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
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
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<IRequestRepository> _requestRepositoryMock = new();
        private readonly Mock<IAccommodationRepository> _accommodationRepositoryMock = new();

        private readonly RequestService _service;

        public RequestServiceTests()
        {
            _service = new RequestService(
                _mapperMock.Object,
                _cacheMock.Object,
                _requestRepositoryMock.Object,
                _accommodationRepositoryMock.Object
            );
        }

        // Helper metoda za postavljanje Id vrednosti pomoću refleksije
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

            // Simulacija cache-a preko GetAsync
            _cacheMock.Setup(c => c.GetAsync(cacheKey, default))
                      .ReturnsAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedRequests)));

            var result = await _service.GetAcceptedByAccommodationId(accommodationId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            _requestRepositoryMock.Verify(r => r.GetAcceptedByAccommodationId(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetAcceptedByAccommodationId_ShouldQueryRepository_WhenCacheEmpty()
        {
            var accommodationId = Guid.NewGuid();
            var request = SetId(new Request { State = RequestState.ACCEPTED }, Guid.NewGuid());
            var requests = new List<Request> { request };

            // Cache je prazna
            _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), default))
                      .ReturnsAsync((byte[])null);

            // Repository vraća listu zahteva
            _requestRepositoryMock.Setup(r => r.GetAcceptedByAccommodationId(accommodationId))
                                  .ReturnsAsync(requests);

            // Mapper vraća DTO listu
            _mapperMock.Setup(m => m.Map<IEnumerable<RequestDto>>(It.IsAny<IEnumerable<Request>>()))
                       .Returns((IEnumerable<Request> src) => src.Select(r => new RequestDto { Id = r.Id }).ToList());

            // SetAsync za cache
            _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(),
                                             It.IsAny<byte[]>(),
                                             It.IsAny<DistributedCacheEntryOptions>(),
                                             default))
                      .Returns(Task.CompletedTask);

            var result = await _service.GetAcceptedByAccommodationId(accommodationId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(),
                                              It.IsAny<byte[]>(),
                                              It.IsAny<DistributedCacheEntryOptions>(),
                                              default), Times.Once);
        }
    }
}