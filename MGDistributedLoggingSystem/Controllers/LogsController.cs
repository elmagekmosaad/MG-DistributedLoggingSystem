using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Entities.Enums;
using MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

namespace MGDistributedLoggingSystem.Controllers
{
    [Route("v1/logs")]
    [ApiController]
    [Authorize(policy: Policies.User)]
    public class LogsController : ControllerBase
    {
        private readonly IDictionary<LogEntryStorageType, ILogEntryStorageService> _logServices;

        public LogsController(
            IDatabaseLogEntryStorageService databaseLogEntryStorageService,
            IFileLogEntryStorageService fileLogEntryStorageService,
            IS3LogEntryStorageService s3LogEntryStorageService,
            IRabbitMQLogEntryStorageService rabbitMQLogEntryStorageService)
        {
            _logServices = new Dictionary<LogEntryStorageType, ILogEntryStorageService>
            {
                { LogEntryStorageType.Database, databaseLogEntryStorageService },
                { LogEntryStorageType.File, fileLogEntryStorageService },
                { LogEntryStorageType.S3, s3LogEntryStorageService },
                { LogEntryStorageType.RabbitMQ, rabbitMQLogEntryStorageService }
            };
        }

        [HttpPost("AddLogEntry")]
        public async Task<IActionResult> AddLog([FromBody] LogEntryAddDto logEntryAddDto)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            Log.Information($"Received log entry: {logEntryAddDto.Message}");

            var logService = GetLogService(logEntryAddDto.StorageType);
            if (logService == null) return  BadRequest("Invalid storage type");

            Log.Information($"Storing log entry for {logEntryAddDto.StorageType} storage type.");

            var result = await logService.StoreLogAsync(logEntryAddDto);
            return Ok(result);
        }

        //[Authorize(policy: Policies.Admin)]
        [HttpGet("GetLogs")]
        public async Task<IActionResult> GetAll(
            [FromQuery] LogEntryFilterDto logEntryFilterDto,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            var logService = GetLogService(logEntryFilterDto.StorageType);
            //if (logService == null) return BadRequest("Invalid storage type");

            Log.Information($"Retrieving logs for {logEntryFilterDto?.StorageType} storage type.");

            var result = await logService.RetrieveLogsAsync(logEntryFilterDto, pageIndex, pageSize);
            return Ok(result);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetLogById(int id, LogEntryStorageType? storageType = LogEntryStorageType.Database)
        {
            if (id <= 0) return BadRequest("Invalid id");

            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            var logService = GetLogService(storageType);

            Log.Information($"Retrieving log for {storageType} storage type and id={id}");

            var result = await logService.GetLogById(id);
            return Ok(result);
        }
        private ILogEntryStorageService GetLogService(LogEntryStorageType? storageType)
        {
            if (!storageType.HasValue)
            {
                storageType = LogEntryStorageType.Database;
            }
            return _logServices.TryGetValue(storageType ?? LogEntryStorageType.Database, out var service) ? service : null;
        }

        private IActionResult ValidateModelState(string message = "Validation failed")
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Log.Warning($"Validation failed: {string.Join(", ", errors)}");

                return BadRequest(new { message, errors });
            }
            return null;
        }
    }
}
