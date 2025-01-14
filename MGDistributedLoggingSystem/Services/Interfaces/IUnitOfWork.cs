using MGDistributedLoggingSystem.Core.IRepository;

namespace MGDistributedLoggingSystem.Services.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ILogEntryRepository LogEntryRepository { get; }
        int SaveChanges();
    }
}
