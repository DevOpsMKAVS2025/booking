using Booking.Domain.Entities;
using Booking.Domain.Entities.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly AuthDBContext authDBContext;
        private readonly DbSet<AppUser> _dbSet;

        public UserRepository(AuthDBContext dbContext)
        {
            authDBContext = dbContext;
            _dbSet = authDBContext.Set<AppUser>();
        }

        public AppUser Get(Guid id)
        {
            AppUser entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            return entity;
        }
    }
}
