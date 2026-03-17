using LogViewer.Core.DTOs;

namespace LogViewer.Core.Interfaces
{
    public interface ILogService
    {
        Task<PagedResult<LogDetailDto>> GetLogsAsync(LogQueryParameters parameters);
        Task<LogDetailDto?> GetLogByIdAsync(long id);
        Task<byte[]> ExportLogsCsvAsync(LogQueryParameters parameters);
        Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync(LogStatsParameters parameters);
        Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(LogStatsTimelineParameters parameters);
        Task<IEnumerable<string>> GetDistinctSourcesAsync();
        Task<IEnumerable<string>> GetDistinctApplicationsAsync();
        Task<DailyStatsDto> GetDailyStatsAsync();
        Task<IEnumerable<LogErrorsAndCriticalsBySource>> GetErrorAndCriticalsBySourceAsync(LogStatsParameters parameters);
    }
}
