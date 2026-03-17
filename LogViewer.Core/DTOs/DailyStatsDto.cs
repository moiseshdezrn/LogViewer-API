namespace LogViewer.Core.DTOs
{
    public class DailyStatsDto
    {
        public int TotalLogs { get; set; }
        public int ErrorCount { get; set; }
        public int CriticalCount { get; set; }
        public double AverageLogsPerHour { get; set; }
    }
}
