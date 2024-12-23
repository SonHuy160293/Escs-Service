namespace Core.Domain.Interfaces
{
    public interface IBaseUnitOfWork : IDisposable
    {

        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
