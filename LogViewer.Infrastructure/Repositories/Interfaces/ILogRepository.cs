using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;

namespace LogViewer.Infrastructure.Repositories.Interfaces
{
    public interface ILogRepository : IBaseRepository<Log>
    {
        Task<(IEnumerable<Log> Items, int TotalCount)> GetPagedAsync(LogQueryParameters parameters);
        Task<Log?> GetByIdAsync(long id);
        Task<IEnumerable<Log>> GetFilteredAsync(LogQueryParameters parameters);
        Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync();
        Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(string groupBy);
        Task<IEnumerable<string>> GetDistinctSourcesAsync();
        Task<IEnumerable<string>> GetDistinctApplicationsAsync();
    }
}
