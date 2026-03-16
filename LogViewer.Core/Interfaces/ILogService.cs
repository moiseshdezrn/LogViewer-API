using LogViewer.Core.DTOs;

namespace LogViewer.Core.Interfaces
{
    public interface ILogService
    {
        Task<PagedResult<LogDetailDto>> GetLogsAsync(LogQueryParameters parameters);
        Task<LogDetailDto?> GetLogByIdAsync(long id);
        Task<byte[]> ExportLogsCsvAsync(LogQueryParameters parameters);
        Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync();
        Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(string groupBy = "day");
        Task<IEnumerable<string>> GetDistinctSourcesAsync();
        Task<IEnumerable<string>> GetDistinctApplicationsAsync();
    }
}
