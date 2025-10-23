using AutoMapper;
using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Booking.BuildingBlocks.Core.UseCases;
using Booking.Domain.Entities.Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using FluentResults;

public class RequestService : BaseService<RequestDto, Request>, IRequestService
{
    private readonly IMapper _mapper;
    private readonly IRequestRepository _repository;
    private readonly IAccommodationRepository _accommodationRepository;

    public RequestService(IMapper mapper, IRequestRepository requestRepository, IAccommodationRepository accommodationRepository)
        : base(mapper)
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

    public async Task<Result<RequestDto>> CreateRequest(RequestDto dto)
    {
        try
        {
            var accommodation = _accommodationRepository.Get(dto.AccommodationId);
            if (accommodation == null)
                return Result.Fail<RequestDto>("Accommodation not found");

            bool isInAvailability = accommodation.Availability.Any(av =>
                dto.StartDate >= av.Duration.From && dto.EndDate <= av.Duration.To);
            if (!isInAvailability)
                return Result.Fail<RequestDto>("Requested period is outside of accommodation availability");

            bool hasConflict = await _repository.HasOverlappingAcceptedRequest(
                dto.AccommodationId, dto.StartDate, dto.EndDate);
            if (hasConflict)
                return Result.Fail<RequestDto>("There is already an accepted request for this accommodation in the selected date range.");

            var request = MapToDomain(dto);
            request.State = accommodation.IsAutoReservation ? RequestState.ACCEPTED : RequestState.PENDING;
            request.IsDeleted = false;

            await _repository.Create(request);
            await _repository.SaveChanges();

            return Result.Ok(_mapper.Map<RequestDto>(request));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>(ex.Message);
        }
    }

    public async Task<Result> DeleteRequest(Guid requestId)
    {
        try
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                return Result.Fail("Request not found or cannot be deleted");

            request.IsDeleted = true;
            await _repository.Update(request);
            await _repository.SaveChanges();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<RequestDto>> GetRequestById(Guid requestId)
    {
        try
        {
            var requests = await _repository.GetById(requestId);
            var dtos = _mapper.Map<IEnumerable<RequestDto>>(requests);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetRequestsByUser(int userId)
    {
        try
        {
            var requests = await _repository.GetByUserId(userId);
            var dtos = _mapper.Map<IEnumerable<RequestDto>>(requests);
            return Result.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetByAccommodationAndUser(Guid accommodationId, int userId)
    {
        try
        {
            var requests = await _repository.GetByAccommodationAndUser(accommodationId, userId);
            var dtos = _mapper.Map<IEnumerable<RequestDto>>(requests);
            return Result.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetRequestsByAccommodation(Guid accommodationId)
    {
        try
        {
            var requests = await _repository.GetByAccommodationId(accommodationId);
            var dtos = _mapper.Map<IEnumerable<RequestDto>>(requests);
            return Result.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }

    public async Task<Result<RequestDto>> ApproveRequest(Guid requestId)
    {
        try
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                return Result.Fail<RequestDto>("Request not found or cannot be approved");

            request.State = RequestState.ACCEPTED;
            await _repository.Update(request);

            var overlappingRequests = await _repository.GetOverlappingRequests(
                request.AccommodationId, request.StartDate, request.EndDate, request.Id);

            foreach (var r in overlappingRequests)
            {
                r.State = RequestState.AUTO_REJECT;
                await _repository.Update(r);
            }

            await _repository.SaveChanges();
            return Result.Ok(_mapper.Map<RequestDto>(request));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>(ex.Message);
        }
    }

    public async Task<Result<RequestDto>> RejectRequest(Guid requestId)
    {
        try
        {
            var request = await _repository.GetById(requestId);
            if (request == null || request.State != RequestState.PENDING)
                return Result.Fail<RequestDto>("Request not found or cannot be rejected");

            request.State = RequestState.USER_REJECT;
            await _repository.Update(request);
            await _repository.SaveChanges();

            return Result.Ok(_mapper.Map<RequestDto>(request));
        }
        catch (Exception ex)
        {
            return Result.Fail<RequestDto>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestWithCancelCountDto>>> GetRequestsWithCancelCountByAccommodation(Guid accommodationId)
    {
        try
        {
            var result = await _repository.GetRequestsWithCancelCountByAccommodation(accommodationId);
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestWithCancelCountDto>>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationId(Guid accommodationId)
    {
        try
        {
            var requests = await _repository.GetAcceptedByAccommodationId(accommodationId);
            return Result.Ok(_mapper.Map<IEnumerable<RequestDto>>(requests));
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetAcceptedByUserId(int userId)
    {
        try
        {
            var requests = await _repository.GetAcceptedByUserId(userId);
            return Result.Ok(_mapper.Map<IEnumerable<RequestDto>>(requests));
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<RequestDto>>> GetAcceptedByAccommodationAndUser(Guid accommodationId, int userId)
    {
        try
        {
            var requests = await _repository.GetAcceptedByAccommodationAndUser(accommodationId, userId);
            return Result.Ok(_mapper.Map<IEnumerable<RequestDto>>(requests));
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RequestDto>>(ex.Message);
        }
    }
}
