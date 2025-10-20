using System.Runtime.CompilerServices;
using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.BuildingBlocks.Core;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities;
using Booking.Domain.Entities.Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.UseCases
{
    public class RequestService : BaseService<RequestDto, Request>, IRequestService
    {
        protected readonly IMapper _mapper;
        protected readonly IRequestRepository _repository;
        protected readonly IAccommodationRepository _accommodationRepository;

        public RequestService(IMapper mapper, IRequestRepository requestRepository, IAccommodationRepository accommodationRepository) : base(mapper)
        {
            _mapper = mapper;
            _repository = requestRepository;
            _accommodationRepository = accommodationRepository;
        }

        public Result<PagedResult<RequestDto>> GetPaged(int page, int pageSize)
        {
            var result = _repository.GetPaged(page, pageSize);
            return MapToDto(result);
        }

        public async Task<Request> CreateRequest(Request request)
        {
            // 1. Load accommodation
            var accommodation = this._accommodationRepository.Get(request.AccommodationId);
            if (accommodation == null)
                throw new Exception("Accommodation not found");

            // 2. Check if is in the available time
            bool isInAvailability = accommodation.Availability.Any(av =>
            request.StartDate >= av.Duration.From && request.EndDate <= av.Duration.To);
            if (!isInAvailability)
                throw new Exception("Requested period is outside of accommodation availability");

            // 3. Check if already have reservation
            bool hasConflict = await _repository.HasOverlappingAcceptedRequest(
                request.AccommodationId,
                request.StartDate,
                request.EndDate);

            if (hasConflict)
                throw new InvalidOperationException("There is already an accepted request for this accommodation in the selected date range.");

            // 4. Create a new request
            if (accommodation.IsAutoReservation)
                request.State = RequestState.ACCEPTED;
            else
                request.State = RequestState.PENDING;

            request.IsDeleted = false;

            await _repository.Create(request);
            await _repository.SaveChanges();

            return request;
        }

        public async Task<bool> DeleteRequest(Guid requestId)
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                return false;

            request.IsDeleted = true;
            await _repository.Update(request);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<Request>> GetRequestsByUser(int userId)
        {
            return await _repository.GetByUserId(userId);
        }

        public async Task<IEnumerable<Request>> GetByAccommodationAndUser(Guid accommodationId, int userId)
        {
            return await _repository.GetByAccommodationAndUser(accommodationId, userId);
        }

        public async Task<IEnumerable<Request>> GetRequestsByAccommodation(Guid accommodationId)
        {
            return await _repository.GetByAccommodationId(accommodationId);
        }

        public async Task<Request> ApproveRequest(Guid requestId)
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                throw new InvalidOperationException("Request not found or cannot be approved.");

            // 1. Accept selected request
            request.State = RequestState.ACCEPTED;
            await _repository.Update(request);

            // 2. Reject all overlapping requests
            var overlappingRequests = await _repository.GetOverlappingRequests(
                request.AccommodationId,
                request.StartDate,
                request.EndDate,
                request.Id
            );

            foreach (var r in overlappingRequests)
            {
                r.State = RequestState.AUTO_REJECT;
                await _repository.Update(r);
            }

            await _repository.SaveChanges();

            return request;
        }

        public async Task<Request> RejectRequest(Guid requestId)
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                throw new InvalidOperationException("Request not found or cannot be rejected.");

            request.State = RequestState.USER_REJECT;
            await _repository.Update(request);
            await _repository.SaveChanges();

            return request;
        }

        public async Task<IEnumerable<RequestWithCancelCountDto>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId)
        {
            return await _repository.GetRequestsWithCancelCountByAccommodation(accommodationId);
        }

        public async Task<IEnumerable<RequestDto>> GetAcceptedByAccommodationId(Guid accommodationId)
        {
            var requests = await _repository.GetAcceptedByAccommodationId(accommodationId);
            return _mapper.Map<IEnumerable<RequestDto>>(requests);
        }

        public async Task<IEnumerable<RequestDto>> GetAcceptedByUserId(int userId)
        {
            var requests = await _repository.GetAcceptedByUserId(userId);
            return _mapper.Map<IEnumerable<RequestDto>>(requests);
        }

        public async Task<IEnumerable<RequestDto>> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId)
        {
            var requests = await _repository.GetAcceptedByAccommodationAndUser(accommodationId, userId);
            return _mapper.Map<IEnumerable<RequestDto>>(requests);
        }

    }
}
