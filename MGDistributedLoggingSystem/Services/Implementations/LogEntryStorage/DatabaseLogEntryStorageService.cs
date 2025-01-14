using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models;
using MGDistributedLoggingSystem.Services.Interfaces;
using MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

namespace MGDistributedLoggingSystem.Services.Implementations.LogEntryStorage
{
    public class DatabaseLogEntryStorageService : IDatabaseLogEntryStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DatabaseLogEntryStorageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<LogEntryResponseDto>> GetLogById(int id)
        {
            try
            {
                var logEntry = await _unitOfWork.LogEntryRepository.GetById(id);
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

        public async Task<BaseResponse<PaginatedList<LogEntryResponseDto>>> RetrieveLogsAsync(LogEntryFilterDto logEntryFilterDto, int pageIndex, int pageSize)
        {
            try
            {
                var query = _unitOfWork.LogEntryRepository.Table.AsQueryable();
                if (logEntryFilterDto != null)
                {
                    var filter = _mapper.Map<LogEntryFilter>(logEntryFilterDto);

                    if (!string.IsNullOrEmpty(filter.Service))
                    {
                        query = query.Where(log => log.Service.ToLower().Contains(filter.Service.ToLower()));
                    }
                    if (filter.Level.HasValue)
                    {
                        query = query.Where(log => log.Level.Equals(filter.Level));
                    }
                    if (filter.StartTime.HasValue)
                    {
                        query = query.Where(log => log.Timestamp >= filter.StartTime.Value);
                    }
                    if (filter.EndTime.HasValue)
                    {
                        query = query.Where(log => log.Timestamp <= filter.EndTime.Value);
                    }
                }

                var totalRecords = await query.CountAsync();
                var logs = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<IEnumerable<LogEntryResponseDto>>(logs);

                Log.Information($"Retrieved {result.Count()} log entries on page {pageIndex} of {totalRecords} total Logs.");

                var paginatedList = new PaginatedList<LogEntryResponseDto>(result, totalRecords, pageIndex, pageSize);

                return new BaseResponse<PaginatedList<LogEntryResponseDto>>($"Retrieved {result.Count()} log entries on page {pageIndex} of {totalRecords} total Logs.", true, paginatedList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving log entries.");
                var emptyPaginatedList = new PaginatedList<LogEntryResponseDto>(Enumerable.Empty<LogEntryResponseDto>(), 0, pageIndex, pageSize);
                return new BaseResponse<PaginatedList<LogEntryResponseDto>>($"Error occurred while retrieving log entries: {ex.Message}", false, emptyPaginatedList);
            }
        }

        public async Task<BaseResponse<string>> StoreLogAsync(LogEntryAddDto logEntryAddDto)
        {
            try
            {
                var entity = _mapper.Map<LogEntry>(logEntryAddDto);
                entity.CreatedDate = DateTime.UtcNow;

                await _unitOfWork.LogEntryRepository.AddAsync(entity);

                var affectedRows = _unitOfWork.SaveChanges();
                if (affectedRows > 0)
                {
                    string successMessage = $"Log [{entity.Service}] saved to {entity.StorageType} successfully";
                    Log.Information(successMessage);

                    return new BaseResponse<string>(successMessage, true, successMessage);
                }
                else
                {
                    string errorMessage = $"Error saving log to file: {affectedRows}";
                    Log.Error(errorMessage);

                    return new BaseResponse<string>(errorMessage, false, null);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error saving log to file: {ex.Message}";
                Log.Error(ex, errorMessage);

                return new BaseResponse<string>(errorMessage, false, null);
            }
        }

    }
}
