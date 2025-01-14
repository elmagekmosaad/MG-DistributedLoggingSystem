using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using MGDistributedLoggingSystem.Data.Entities;

namespace MGDistributedLoggingSystem.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
     
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }
    
    }
}
