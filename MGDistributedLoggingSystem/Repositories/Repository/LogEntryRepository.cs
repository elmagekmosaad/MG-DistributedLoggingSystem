using MGDistributedLoggingSystem.Data.Context;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Core.IRepository;

namespace MGDistributedLoggingSystem.Core.Repository
{
    public class LogEntryRepository : GenericRepository<LogEntry>, ILogEntryRepository
    {
        public LogEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
