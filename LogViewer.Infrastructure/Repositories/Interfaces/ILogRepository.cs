using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;

namespace LogViewer.Infrastructure.Repositories.Interfaces
{
    public interface ILogRepository : IBaseRepository<Log>
    {
        Task<(IEnumerable<Log> Items, int TotalCount)> GetPagedAsync(LogQueryParameters parameters);
        Task<Log?> GetByIdAsync(long id);
        Task<IEnumerable<Log>> GetFilteredAsync(LogQueryParameters parameters);
        Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync(LogStatsParameters parameters);
        Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(LogStatsTimelineParameters parameters);
        Task<IEnumerable<LogErrorsAndCriticalsBySource>> GetErrorsAndCriticalsBySource(LogStatsParameters parameters, int take = 10);
        Task<IEnumerable<string>> GetDistinctSourcesAsync();
        Task<IEnumerable<string>> GetDistinctApplicationsAsync();
        Task<DailyStatsDto> GetDailyStatsAsync();
    }
}
