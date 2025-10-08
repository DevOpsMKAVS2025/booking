namespace Booking.BuildingBlocks.Core.UseCases
{
    public interface ICrudRepository<TEntity> where TEntity : Entity
    {
        PagedResult<TEntity> GetPaged(int page, int pageSize);
        TEntity Get(Guid id);
        TEntity Create(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(Guid id);
    }
}
