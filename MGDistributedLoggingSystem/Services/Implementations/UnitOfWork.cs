using MGDistributedLoggingSystem.Data.Context;
using MGDistributedLoggingSystem.Core.Repository;
using MGDistributedLoggingSystem.Core.IRepository;
using MGDistributedLoggingSystem.Services.Interfaces;

namespace MGDistributedLoggingSystem.Services.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            LogEntryRepository = new LogEntryRepository(context);
        }
        public ILogEntryRepository LogEntryRepository { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
