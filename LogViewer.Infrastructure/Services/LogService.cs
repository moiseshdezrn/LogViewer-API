using System.Text;
using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;
using LogViewer.Core.Interfaces;
using LogViewer.Infrastructure.Repositories.Interfaces;

namespace LogViewer.Infrastructure.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<PagedResult<LogDetailDto>> GetLogsAsync(LogQueryParameters parameters)
        {
            var (items, totalCount) = await _logRepository.GetPagedAsync(parameters);

            return new PagedResult<LogDetailDto>
            {
                Items = items.Select(MapToDetailDto),
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<LogDetailDto?> GetLogByIdAsync(long id)
        {
            var log = await _logRepository.GetByIdAsync(id);
            return log is null ? null : MapToDetailDto(log);
        }

        public async Task<byte[]> ExportLogsCsvAsync(LogQueryParameters parameters)
        {
            var logs = await _logRepository.GetFilteredAsync(parameters);

            var sb = new StringBuilder();
            sb.AppendLine("Id,Timestamp,Level,Message,Source,ExceptionMessage,StackTrace,UserId,MachineName,RequestPath,RequestMethod,CorrelationId,ThreadId,ApplicationName");

            foreach (var log in logs)
            {
                sb.AppendLine(string.Join(",",
                    log.Id,
                    CsvEscape(log.Timestamp.ToString("o")),
                    CsvEscape(log.Level),
                    CsvEscape(log.Message),
                    CsvEscape(log.Source),
                    CsvEscape(log.ExceptionMessage),
                    CsvEscape(log.StackTrace),
                    CsvEscape(log.UserId),
                    CsvEscape(log.MachineName),
                    CsvEscape(log.RequestPath),
                    CsvEscape(log.RequestMethod),
                    log.CorrelationId?.ToString(),
                    log.ThreadId?.ToString(),
                    CsvEscape(log.ApplicationName)));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<IEnumerable<LogLevelStatsDto>> GetLevelStatsAsync()
        {
            return await _logRepository.GetLevelStatsAsync();
        }

        public async Task<IEnumerable<LogTimelineStatsDto>> GetTimelineStatsAsync(string groupBy = "day")
        {
            return await _logRepository.GetTimelineStatsAsync(groupBy);
        }

        public async Task<IEnumerable<string>> GetDistinctSourcesAsync()
        {
            return await _logRepository.GetDistinctSourcesAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctApplicationsAsync()
        {
            return await _logRepository.GetDistinctApplicationsAsync();
        }

        private static LogDetailDto MapToDetailDto(Log l)
        {
            return new LogDetailDto
            {
                Id = l.Id,
                Timestamp = l.Timestamp,
                Level = l.Level,
                Message = l.Message,
                Source = l.Source,
                ExceptionMessage = l.ExceptionMessage,
                StackTrace = l.StackTrace,
                UserId = l.UserId,
                MachineName = l.MachineName,
                RequestPath = l.RequestPath,
                RequestMethod = l.RequestMethod,
                CorrelationId = l.CorrelationId,
                ThreadId = l.ThreadId,
                ApplicationName = l.ApplicationName
            };
        }

        private static string CsvEscape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }
    }
}
