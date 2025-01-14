using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MGDistributedLoggingSystem.Data.Entities.Enums;

namespace MGDistributedLoggingSystem.Models
{
    public class LogEntryFilter
    {
        //- `service` (optional): Filter logs by service name.
        //- `level` (optional): Filter logs by severity level(`info`, `warning`, `error`).
        //- `start_time` (optional): Start of the time range for logs.
        //- `end_time` (optional): End of the time range for logs.
        [Description("Database | File | S3 | RabbitMQ")]
        [EnumDataType(typeof(LogEntryStorageType))]
        public LogEntryStorageType StorageType { get; set; }
        [DisplayName("Name of the service")]
        public string? Service { get; set; }
        [Description("Info | Warning | Error ")]
        [EnumDataType(typeof(LogEntryLevel))]
        public LogEntryLevel? Level { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}