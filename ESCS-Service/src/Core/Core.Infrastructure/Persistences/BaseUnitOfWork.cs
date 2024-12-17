using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistences
{
    public class BaseUnitOfWork : IBaseUnitOfWork
    {
        private readonly DbContext _context;


        public BaseUnitOfWork(DbContext context, IServiceProvider provider)
        {
            _context = context;

        }


        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
