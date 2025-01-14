using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MGDistributedLoggingSystem.Data.Entities.Enums;

namespace MGDistributedLoggingSystem.Data.Entities
{
    [Table("LogEntries", Schema = "Logging")]
    public class LogEntry
    {
        [Key]
        [JsonIgnore]//ignore this property in serialization in file
        public int Id { get; set; }

        [Description("Database | File | S3 | RabbitMQ")]
        [EnumDataType(typeof(LogEntryStorageType))]
        public LogEntryStorageType StorageType { get; set; }

        [Required]
        [DisplayName("Name of the service")]
        public string Service { get; set; }
        [Required]
        [Description("Info | Warning | Error ")]
        [EnumDataType(typeof(LogEntryLevel))]
        public LogEntryLevel Level { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}
