using AutoMapper;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models;
using MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MGDistributedLoggingSystem.Services.Interfaces;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

namespace MGDistributedLoggingSystem.Services.Implementations.LogEntryStorage
{
    public class FileLogEntryStorageService : IFileLogEntryStorageService
    {
        private string _fileStoragePath;
        private string _jsonFileStoragePath;
        private readonly IMapper _mapper;

        public FileLogEntryStorageService(IConfiguration configuration, IMapper mapper)
        {
            getFileStoragePath(configuration);

            _jsonFileStoragePath = Path.Combine(_fileStoragePath, "logs.json");

            _mapper = mapper;
        }

        private void getFileStoragePath(IConfiguration configuration)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _fileStoragePath = configuration["Storage:LocalFileSystem:Windows"];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _fileStoragePath = configuration["Storage:LocalFileSystem:Linux"];
            }
            else
            {
                throw new NotSupportedException("The current operating system is not supported.");
            }

            if (string.IsNullOrEmpty(_fileStoragePath))
            {
                _fileStoragePath = "/var/logs/distributed_system";
            }
        }
        private async Task<List<LogEntry>> ReadLogsFromFileAsync()
        {
            try
            {
                if (!Directory.Exists(_fileStoragePath))
                {
                    try
                    {
                        Directory.CreateDirectory(_fileStoragePath);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Log.Error(ex, "Unauthorized access while creating directory: {DirectoryPath}", _fileStoragePath);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error while creating directory: {DirectoryPath}", _fileStoragePath);
                        throw;
                    }
                }

                if (!File.Exists(_jsonFileStoragePath))
                {
                    return new List<LogEntry>();
                }

                var content = await File.ReadAllTextAsync(_jsonFileStoragePath);
                return JsonSerializer.Deserialize<List<LogEntry>>(content) ?? new List<LogEntry>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error reading logs from file");
                throw;
            }
        }
        private async Task SaveLogsToFileAsync(List<LogEntry> logs)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };

                var logContent = JsonSerializer.Serialize(logs, options);

                await File.WriteAllTextAsync(_jsonFileStoragePath, logContent, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving logs to file");
                throw;
            }
        }
        public async Task<BaseResponse<LogEntryResponseDto>> GetLogById(int id)
        {
            try
            {
                var logs = await ReadLogsFromFileAsync();
                
                var logEntry = logs.FirstOrDefault(log => log.Id == id);
                if (logEntry == null)
                {
                    string errorMessage = $"Log entry with ID {id} not found.";
                    Log.Error(errorMessage);

                    return new BaseResponse<LogEntryResponseDto>(errorMessage, false, null);
                }

                var logEntryDto = _mapper.Map<LogEntryResponseDto>(logEntry);

                string successMessage = $"Log entry with ID {id} retrieved successfully.";
                Log.Information(successMessage);

                return new BaseResponse<LogEntryResponseDto>(successMessage, true, logEntryDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while retrieving log entry with ID {id}.");
                return new BaseResponse<LogEntryResponseDto>($"Error occurred while retrieving log entry: {ex.Message}", false, null);
            }
        }

        public async Task<BaseResponse<string>> StoreLogAsync(LogEntryAddDto logEntryAddDto)
        {
            try
            {
                var entity = _mapper.Map<LogEntry>(logEntryAddDto);
                entity.CreatedDate = DateTime.UtcNow;

                List<LogEntry> logs = await ReadLogsFromFileAsync();

                logs.Add(entity);

                await SaveLogsToFileAsync(logs);

                string successMessage = $"Log [{entity.Service}] saved to {entity.StorageType} successfully";
                Log.Information(successMessage);

                return new BaseResponse<string>(successMessage, true, successMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error saving log to file";
                Log.Error(ex, errorMessage);

                return new BaseResponse<string>($"{errorMessage}: {ex.Message}", false, null);
            }
        }

        public async Task<BaseResponse<PaginatedList<LogEntryResponseDto>>> RetrieveLogsAsync(LogEntryFilterDto logEntryFilterDto, int pageIndex, int pageSize)
        {
            try
            {
                var logs = await ReadLogsFromFileAsync();

                if (logEntryFilterDto != null)
                {
                    var filter = _mapper.Map<LogEntryFilter>(logEntryFilterDto);

                    //if (filter.StorageType.HasValue)
                    //{
                    //    logs = logs.Where(log => log.StorageType.Equals(filter.StorageType)).ToList();
                    //}
                    if (!string.IsNullOrEmpty(filter.Service))
                    {
                        logs = logs.Where(log => log.Service.Contains(filter.Service, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (filter.Level.HasValue)
                    {
                        logs = logs.Where(log => log.Level.Equals(filter.Level)).ToList();
                    }
                    if (filter.StartTime.HasValue)
                    {
                        logs = logs.Where(log => log.Timestamp >= filter.StartTime.Value).ToList();
                    }
                    if (filter.EndTime.HasValue)
                    {
                        logs = logs.Where(log => log.Timestamp <= filter.EndTime.Value).ToList();
                    }
                }

                var totalLogs = logs.Count;
                var paginatedLogs = logs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var result = _mapper.Map<List<LogEntryResponseDto>>(paginatedLogs);

                var paginatedList = new PaginatedList<LogEntryResponseDto>(result, totalLogs, pageIndex, pageSize);

                string message = $"Retrieved {result.Count} log entries on page {pageIndex} of {totalLogs} total Logs";

                Log.Information(message);

                return new BaseResponse<PaginatedList<LogEntryResponseDto>>(message, true, paginatedList);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while retrieving log entries";
                Log.Error(ex, errorMessage);

                return new BaseResponse<PaginatedList<LogEntryResponseDto>>(errorMessage, false, null);
            }

        }
    }
}
