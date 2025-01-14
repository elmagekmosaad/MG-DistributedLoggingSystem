using MGDistributedLoggingSystem.Helpers;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;

namespace MGDistributedLoggingSystem.Services.Interfaces.LogEntryStorage
{
    public interface ILogEntryStorageService
    {
        Task<BaseResponse<LogEntryResponseDto>> GetLogById(int id);
        Task<BaseResponse<PaginatedList<LogEntryResponseDto>>> RetrieveLogsAsync(LogEntryFilterDto logEntryFilterDto, int pageIndex, int pageSize);
        Task<BaseResponse<string>> StoreLogAsync(LogEntryAddDto logEntry);
    }
}