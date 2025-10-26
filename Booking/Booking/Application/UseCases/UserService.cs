using AutoMapper;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;

namespace Booking.Application.UseCases
{
    public class UserService : IUserService
    {
        protected readonly IUserRepository _repository;

        public UserService(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public AppUser Get(Guid id)
        {
            return _repository.Get(id);
        }
    }
}
