using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;
using LogViewer.Infrastructure.Repositories.Interfaces;
using LogViewer.Infrastructure.Services;
using Moq;

namespace LogViewer.Tests
{
    public class LogServiceTests
    {
        private readonly Mock<ILogRepository> _mockLogRepository;
        private readonly LogService _logService;

        public LogServiceTests()
        {
            _mockLogRepository = new Mock<ILogRepository>();
            _logService = new LogService(_mockLogRepository.Object);
        }

        [Fact]
        public async Task GetLogsAsync_WithValidParameters_ReturnsPagedResult()
        {
            // Arrange
            var parameters = new LogQueryParameters { Page = 1, PageSize = 25 };
            var logs = new List<Log>
            {
                new Log
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Level = "Information",
                    Message = "Test log",
                    Source = "TestSource",
                    MachineName = "TestMachine",
                    ApplicationName = "TestApp"
                }
            };

            _mockLogRepository.Setup(r => r.GetPagedAsync(parameters))
                .ReturnsAsync((logs, 1));

            // Act
            var result = await _logService.GetLogsAsync(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(25, result.PageSize);
        }

        [Fact]
        public async Task GetLogByIdAsync_WithExistingId_ReturnsLogDetailDto()
        {
            // Arrange
            var logId = 1L;
            var log = new Log
            {
                Id = logId,
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Message = "Test error",
                Source = "TestSource",
                MachineName = "TestMachine",
                ApplicationName = "TestApp"
            };

            _mockLogRepository.Setup(r => r.GetByIdAsync(logId))
                .ReturnsAsync(log);

            // Act
            var result = await _logService.GetLogByIdAsync(logId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(logId, result.Id);
            Assert.Equal(log.Level, result.Level);
            Assert.Equal(log.Message, result.Message);
        }

        [Fact]
        public async Task GetLogByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var logId = 999L;
            _mockLogRepository.Setup(r => r.GetByIdAsync(logId))
                .ReturnsAsync((Log?)null);

            // Act
            var result = await _logService.GetLogByIdAsync(logId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ExportLogsCsvAsync_WithLogs_ReturnsCsvBytes()
        {
            // Arrange
            var parameters = new LogQueryParameters();
            var logs = new List<Log>
            {
                new Log
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Level = "Information",
                    Message = "Test log",
                    Source = "TestSource",
                    MachineName = "TestMachine",
                    ApplicationName = "TestApp"
                }
            };

            _mockLogRepository.Setup(r => r.GetFilteredAsync(parameters))
                .ReturnsAsync(logs);

            // Act
            var result = await _logService.ExportLogsCsvAsync(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("Id,Timestamp,Level,Message", csv);
            Assert.Contains("Test log", csv);
        }

        [Fact]
        public async Task ExportLogsCsvAsync_EscapesCommasInValues()
        {
            // Arrange
            var parameters = new LogQueryParameters();
            var logs = new List<Log>
            {
                new Log
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Level = "Information",
                    Message = "Test, with, commas",
                    Source = "TestSource",
                    MachineName = "TestMachine",
                    ApplicationName = "TestApp"
                }
            };

            _mockLogRepository.Setup(r => r.GetFilteredAsync(parameters))
                .ReturnsAsync(logs);

            // Act
            var result = await _logService.ExportLogsCsvAsync(parameters);

            // Assert
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("\"Test, with, commas\"", csv);
        }

        [Fact]
        public async Task ExportLogsCsvAsync_EscapesQuotesInValues()
        {
            // Arrange
            var parameters = new LogQueryParameters();
            var logs = new List<Log>
            {
                new Log
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Level = "Information",
                    Message = "Test \"quoted\" message",
                    Source = "TestSource",
                    MachineName = "TestMachine",
                    ApplicationName = "TestApp"
                }
            };

            _mockLogRepository.Setup(r => r.GetFilteredAsync(parameters))
                .ReturnsAsync(logs);

            // Act
            var result = await _logService.ExportLogsCsvAsync(parameters);

            // Assert
            var csv = System.Text.Encoding.UTF8.GetString(result);
            Assert.Contains("\"\"", csv);
        }

        [Fact]
        public async Task GetLevelStatsAsync_ReturnsStatsFromRepository()
        {
            // Arrange
            var expectedStats = new List<LogLevelStatsDto>
            {
                new LogLevelStatsDto { Level = "Error", Count = 10 },
                new LogLevelStatsDto { Level = "Warning", Count = 20 }
            };
            var parameters = new LogStatsParameters ();
            _mockLogRepository.Setup(r => r.GetLevelStatsAsync(parameters))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _logService.GetLevelStatsAsync(parameters);

            // Assert
            Assert.Equal(expectedStats, result);
        }

        [Fact]
        public async Task GetTimelineStatsAsync_WithDefaultGroupBy_ReturnsStats()
        {
            // Arrange
            var expectedStats = new List<LogTimelineStatsDto>
            {
                new LogTimelineStatsDto { Period = DateTime.UtcNow, Count = 50 }
            };
            var parameters = new LogStatsTimelineParameters ();
            _mockLogRepository.Setup(r => r.GetTimelineStatsAsync(parameters))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _logService.GetTimelineStatsAsync(parameters);

            // Assert
            Assert.Equal(expectedStats, result);
        }

        [Fact]
        public async Task GetDistinctSourcesAsync_ReturnsSourcesFromRepository()
        {
            // Arrange
            var expectedSources = new List<string> { "Source1", "Source2" };

            _mockLogRepository.Setup(r => r.GetDistinctSourcesAsync())
                .ReturnsAsync(expectedSources);

            // Act
            var result = await _logService.GetDistinctSourcesAsync();

            // Assert
            Assert.Equal(expectedSources, result);
        }

        [Fact]
        public async Task GetDistinctApplicationsAsync_ReturnsApplicationsFromRepository()
        {
            // Arrange
            var expectedApps = new List<string> { "App1", "App2" };

            _mockLogRepository.Setup(r => r.GetDistinctApplicationsAsync())
                .ReturnsAsync(expectedApps);

            // Act
            var result = await _logService.GetDistinctApplicationsAsync();

            // Assert
            Assert.Equal(expectedApps, result);
        }
    }
}
