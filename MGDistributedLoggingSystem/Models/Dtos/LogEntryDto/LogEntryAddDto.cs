using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MGDistributedLoggingSystem.Data.Entities.Enums;

namespace MGDistributedLoggingSystem.Models.Dtos.LogEntryDto
{
    public class LogEntryAddDto
    {
        [Required]
        [EnumDataType(typeof(LogEntryStorageType))]
        public LogEntryStorageType StorageType { get; set; }
        [Required]
        public string Service { get; set; }
        [Required]
        [EnumDataType(typeof(LogEntryLevel))]
        public LogEntryLevel Level { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }


    }
}
