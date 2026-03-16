namespace LogViewer.Core.DTOs
{
    public class LogQueryParameters
    {
        private int _pageSize = 25;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value < 1 ? 1 : value;
        }

        public string SortBy { get; set; } = "Timestamp";
        public string SortDirection { get; set; } = "desc";
        public string[]? Level { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Source { get; set; }
        public string? Application { get; set; }
        public string? Search { get; set; }
        public Guid? CorrelationId { get; set; }
    }
}
