using Microsoft.EntityFrameworkCore;
using MGDistributedLoggingSystem.Data.Context;
using MGDistributedLoggingSystem.Core.IRepository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MGDistributedLoggingSystem.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<T> Table => _context.Set<T>();

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}