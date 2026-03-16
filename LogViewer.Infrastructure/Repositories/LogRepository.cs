using System.Linq.Expressions;
using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;
using LogViewer.Infrastructure.DatabaseContext;
using LogViewer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LogViewer.Infrastructure.Repositories
{
    public class LogRepository : BaseRepository<Log, LogDbContext>, ILogRepository
    {
        private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "Id", "Timestamp", "Level", "Message", "Source", "MachineName", "ApplicationName"
        };

        public LogRepository(LogDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Log> Items, int TotalCount)> GetPagedAsync(LogQueryParameters parameters)
        {
            var query = BuildFilteredQuery(parameters);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Log?> GetByIdAsync(long id)
        {
            return await _context.Logs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Log>> GetFilteredAsync(LogQueryParameters parameters)
        {
            return await BuildFilteredQuery(parameters).ToListAsync();
        }

        public async Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync()
        {
            return await _context.Logs
                .AsNoTracking()
                .GroupBy(l => l.Level)
                .Select(g => new LogLevelStatsDto
                {
                    Level = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(string groupBy)
        {
            if (string.Equals(groupBy, "hour", StringComparison.OrdinalIgnoreCase))
            {
                return await _context.Logs
                    .AsNoTracking()
                    .GroupBy(l => new { l.Timestamp.Date, l.Timestamp.Hour })
                    .Select(g => new LogTimelineStatsDto
                    {
                        Period = g.Key.Date.AddHours(g.Key.Hour),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Period)
                    .ToListAsync();
            }

            return await _context.Logs
                .AsNoTracking()
                .GroupBy(l => l.Timestamp.Date)
                .Select(g => new LogTimelineStatsDto
                {
                    Period = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Period)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctSourcesAsync()
        {
            return await _context.Logs
                .AsNoTracking()
                .Select(l => l.Source)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctApplicationsAsync()
        {
            return await _context.Logs
                .AsNoTracking()
                .Select(l => l.ApplicationName)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        private IQueryable<Log> BuildFilteredQuery(LogQueryParameters parameters)
        {
            var query = _context.Logs.AsNoTracking().AsQueryable();

            if (parameters.Level is { Length: > 0 })
                query = query.Where(l => parameters.Level.Contains(l.Level));

            if (parameters.StartDate.HasValue)
                query = query.Where(l => l.Timestamp >= parameters.StartDate.Value);

            if (parameters.EndDate.HasValue)
                query = query.Where(l => l.Timestamp <= parameters.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(parameters.Source))
                query = query.Where(l => l.Source == parameters.Source);

            if (!string.IsNullOrWhiteSpace(parameters.Application))
                query = query.Where(l => l.ApplicationName == parameters.Application);

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search;
                query = query.Where(l =>
                    l.Message.Contains(search) ||
                    (l.ExceptionMessage != null && l.ExceptionMessage.Contains(search)));
            }

            if (parameters.CorrelationId.HasValue)
                query = query.Where(l => l.CorrelationId == parameters.CorrelationId.Value);

            query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

            return query;
        }

        private static IQueryable<Log> ApplySorting(IQueryable<Log> query, string sortBy, string sortDirection)
        {
            if (!AllowedSortColumns.Contains(sortBy))
                sortBy = "Timestamp";

            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            Expression<Func<Log, object>> keySelector = sortBy.ToLowerInvariant() switch
            {
                "id" => l => l.Id,
                "level" => l => l.Level,
                "message" => l => l.Message,
                "source" => l => l.Source,
                "machinename" => l => l.MachineName,
                "applicationname" => l => l.ApplicationName,
                _ => l => l.Timestamp
            };

            return isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }
    }
}
