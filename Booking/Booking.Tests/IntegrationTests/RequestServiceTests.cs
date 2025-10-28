using Booking.Application.Dtos;
using Booking.BuildingBlocks.Core;
using Booking.Domain.Entities;
using Booking.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Booking.Tests.IntegrationTests
{
    public class RequestServiceTest : IntegrationTestBase
    {
        [Fact]
        public async Task CreateRequest_ShouldPersistInDatabase()
        {
            // Seed Accommodation
            Guid accommodationId = Guid.NewGuid();
            Guid guestId = Guid.NewGuid();

            Accommodation accommodation = new Accommodation
            {
                Name = "Test Accommodation",
                Location = "Test City",
                MinGuestNumber = 1,
                MaxGuestNumber = 5,
                Conveniences = new List<ConvenieceType> { ConvenieceType.WIFI },
                Photos = new List<string> { "photo1.jpg" },
                Availability = new List<Availability>
                {
                    new Availability
                    {
                        Duration = new DateRange
                        {
                            From = DateTime.UtcNow,
                            To = DateTime.UtcNow.AddDays(10)
                        }
                    }
                },
                Prices = new List<Price>
                {
                    new Price
                    {
                        Amount = 100,
                        Duration = new DateRange
                        {
                            From = DateTime.UtcNow,
                            To = DateTime.UtcNow.AddDays(10)
                        },
                        PriceType = PriceType.PER_GUEST
                    }
                },
                GlobalPrice = 100,
                OwnerId = Guid.NewGuid()
            };

            typeof(Entity).GetProperty("Id")!.SetValue(accommodation, accommodationId);

            await SeedDatabaseAsync<BookingDbContext>(async db =>
            {
                db.Accommodations.Add(accommodation);
                await Task.CompletedTask;
            });

            var requestDto = new Application.Dtos.RequestDto
            {
                AccommodationId = accommodationId,
                GuestId = guestId,
                Accommodation = accommodation.Name,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                GuestNum = 2
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/request", requestDto);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Approve failed: {body}");
            }
            // Assert
            response.EnsureSuccessStatusCode();

            var dbContext = GetDbContext<BookingDbContext>();
            var requestInDb = await dbContext.Requests.FirstOrDefaultAsync(r =>
                r.AccommodationId == accommodationId && r.GuestId == guestId);

            Assert.NotNull(requestInDb);
            Assert.Equal(requestDto.GuestNum, requestInDb.GuestNum);
        }

        [Fact]
        public async Task ApproveRequest_ShouldAutoRejectOverlappingRequests()
        {
            Guid accommodationId = Guid.NewGuid();
            Guid request1Id = Guid.NewGuid();
            Guid request2Id = Guid.NewGuid();
            Guid guest1Id = Guid.NewGuid();
            Guid guest2Id = Guid.NewGuid();

            var accommodation = new Accommodation
            {
                Name = "Test Accommodation",
                Location = "Test City",
                MinGuestNumber = 1,
                MaxGuestNumber = 5,
                Conveniences = new List<ConvenieceType> { ConvenieceType.WIFI },
                Photos = new List<string> { "photo1.jpg" },
                Availability = new List<Availability>
                {
                    new Availability
                    {
                        Duration = new DateRange
                        {
                            From = DateTime.UtcNow,
                            To = DateTime.UtcNow.AddDays(10)
                        }
                    }
                },
                Prices = new List<Price>
                {
                new Price
                {
                    Amount = 100,
                    Duration = new DateRange
                    {
                        From = DateTime.UtcNow,
                        To = DateTime.UtcNow.AddDays(10)
                    },
                    PriceType = PriceType.PER_GUEST
                }
            },
                GlobalPrice = 100,
                OwnerId = Guid.NewGuid()
            };

            typeof(Entity).GetProperty("Id")!.SetValue(accommodation, accommodationId);

            await SeedDatabaseAsync<BookingDbContext>(async db =>
            {
                db.Accommodations.Add(accommodation);
                await db.SaveChangesAsync();
            });

            await SeedDatabaseAsync<BookingDbContext>(async db =>
            {
                var request1 = new Request
                {
                    AccommodationId = accommodationId,
                    GuestId = guest1Id,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(2),
                    GuestNum = 2,
                    State = RequestState.PENDING
                };
                typeof(Entity).GetProperty("Id")!.SetValue(request1, request1Id);

                var request2 = new Request
                {
                    AccommodationId = accommodationId,
                    GuestId = guest2Id,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(2),
                    GuestNum = 2,
                    State = RequestState.PENDING
                };
                typeof(Entity).GetProperty("Id")!.SetValue(request2, request2Id);

                db.Requests.AddRange(request1, request2);
                await db.SaveChangesAsync();
            });

            // Approve first request
            var dbCtx = GetDbContext<BookingDbContext>();
            var pendingRequest = await dbCtx.Requests.FirstAsync(r => r.Id == request1Id);

            var approveResponse = await _httpClient.PostAsync($"/api/request/approve/{pendingRequest.Id}", null);
            approveResponse.EnsureSuccessStatusCode();

            using var refreshedCtx = GetDbContext<BookingDbContext>();
            var first = await refreshedCtx.Requests.FirstAsync(r => r.Id == request1Id);
            var second = await refreshedCtx.Requests.FirstAsync(r => r.Id == request2Id);

            Assert.Equal(RequestState.ACCEPTED, first.State);
            Assert.Equal(RequestState.AUTO_REJECT, second.State);
        }

        [Fact]
        public async Task RejectRequest_ShouldSetStatusToRejected()
        {
            // Arrange
            Guid accommodationId = Guid.NewGuid();
            Guid requestId = Guid.NewGuid();
            Guid guestId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Seed accommodation
            var accommodation = new Accommodation
            {
                Name = "Reject Test Acc",
                Location = "City",
                MinGuestNumber = 1,
                MaxGuestNumber = 5,
                Conveniences = new List<ConvenieceType> { ConvenieceType.WIFI },
                Photos = new List<string> { "p.jpg" },
                Availability = new List<Availability>
            {
                new Availability
                {
                    Duration = new DateRange
                    {
                        From = now,
                        To = now.AddDays(5)
                    }
                }
            },
                    Prices = new List<Price>
            {
                new Price
                {
                    Amount = 80,
                    Duration = new DateRange { From = now, To = now.AddDays(5) },
                    PriceType = PriceType.PER_UNIT
                }
            },
                GlobalPrice = 80,
                OwnerId = Guid.NewGuid()
            };

            typeof(Entity).GetProperty("Id")!.SetValue(accommodation, accommodationId);

            await SeedDatabaseAsync<BookingDbContext>(async db =>
            {
                db.Accommodations.Add(accommodation);
                await db.SaveChangesAsync();
            });

            // Seed request
            await SeedDatabaseAsync<BookingDbContext>(async db =>
            {
                var request = new Request
                {
                    AccommodationId = accommodationId,
                    GuestId = guestId,
                    StartDate = now.AddDays(1),
                    EndDate = now.AddDays(2),
                    GuestNum = 2,
                    State = RequestState.ACCEPTED
                };

                typeof(Entity).GetProperty("Id")!.SetValue(request, requestId);

                db.Requests.Add(request);
                await db.SaveChangesAsync();
            });

            // Act — reject request via API
            var response = await _httpClient.PostAsync($"/api/request/reject/{requestId}", null);
            response.EnsureSuccessStatusCode();

            // Assert — refresh DB context
            using var refreshedCtx = GetDbContext<BookingDbContext>();
            var updatedReq = await refreshedCtx.Requests.FirstOrDefaultAsync(r => r.Id == requestId);

            Assert.NotNull(updatedReq);
            Assert.Equal(RequestState.USER_REJECT, updatedReq!.State);
        }
    }
}
