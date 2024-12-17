namespace Core.Domain.Interfaces
{
    public interface IBaseUnitOfWork : IDisposable
    {

        Task SaveChangesAsync();
    }
}
