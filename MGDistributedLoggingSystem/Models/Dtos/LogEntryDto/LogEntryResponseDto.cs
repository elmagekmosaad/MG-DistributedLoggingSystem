using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;
using MGDistributedLoggingSystem.Data.Entities.Enums;
using MGDistributedLoggingSystem.Data.Entities;

namespace MGDistributedLoggingSystem.Models.Dtos.LogEntryDto
{
    public class LogEntryResponseDto : LogEntry
    {
        public new int Id { get; set; }
    }
}
