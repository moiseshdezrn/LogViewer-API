namespace LogViewer.Core.DTOs
{
    public class LogDetailDto
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? UserId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public Guid? CorrelationId { get; set; }
        public int? ThreadId { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
    }
}
