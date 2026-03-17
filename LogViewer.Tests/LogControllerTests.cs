using LogViewer.Core.DTOs;
using LogViewer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UnosquareTechnicalExam.Controllers;

namespace LogViewer.Tests
{
    public class LogControllerTests
    {
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<ILogger<LogController>> _mockLogger;
        private readonly LogController _controller;

        public LogControllerTests()
        {
            _mockLogService = new Mock<ILogService>();
            _mockLogger = new Mock<ILogger<LogController>>();
            _controller = new LogController(_mockLogService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetLogs_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var parameters = new LogQueryParameters { Page = 1, PageSize = 25 };
            var expectedResult = new PagedResult<LogDetailDto>
            {
                Items = new List<LogDetailDto>
                {
                    new LogDetailDto { Id = 1, Level = "Information", Message = "Test log" }
                },
                TotalCount = 1,
                Page = 1,
                PageSize = 25
            };

            _mockLogService.Setup(s => s.GetLogsAsync(parameters))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetLogs(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
            _mockLogService.Verify(s => s.GetLogsAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task GetLogById_WithExistingId_ReturnsOkResult()
        {
            // Arrange
            var logId = 1L;
            var expectedLog = new LogDetailDto
            {
                Id = logId,
                Level = "Error",
                Message = "Test error"
            };

            _mockLogService.Setup(s => s.GetLogByIdAsync(logId))
                .ReturnsAsync(expectedLog);

            // Act
            var result = await _controller.GetLogById(logId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedLog, okResult.Value);
        }

        [Fact]
        public async Task GetLogById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var logId = 999L;
            _mockLogService.Setup(s => s.GetLogByIdAsync(logId))
                .ReturnsAsync((LogDetailDto?)null);

            // Act
            var result = await _controller.GetLogById(logId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ExportLogs_WithValidParameters_ReturnsFileResult()
        {
            // Arrange
            var parameters = new LogQueryParameters();
            var csvBytes = System.Text.Encoding.UTF8.GetBytes("Id,Timestamp,Level\n1,2026-03-15,Info");

            _mockLogService.Setup(s => s.ExportLogsCsvAsync(parameters))
                .ReturnsAsync(csvBytes);

            // Act
            var result = await _controller.ExportLogs(parameters);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("logs_export.csv", fileResult.FileDownloadName);
            Assert.Equal(csvBytes, fileResult.FileContents);
        }

        [Fact]
        public async Task GetLevelStats_ReturnsOkResultWithStats()
        {
            // Arrange
            var expectedStats = new List<LogLevelStatsDto>
            {
                new LogLevelStatsDto { Level = "Error", Count = 10 },
                new LogLevelStatsDto { Level = "Warning", Count = 20 }
            };
            var parameters = new LogStatsParameters {  };
            _mockLogService.Setup(s => s.GetLevelStatsAsync(parameters))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GetLevelStats(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedStats, okResult.Value);
        }

        [Fact]
        public async Task GetTimelineStats_WithDefaultGroupBy_ReturnsOkResult()
        {
            // Arrange
            var expectedStats = new List<LogTimelineStatsDto>
            {
                new LogTimelineStatsDto { Period = DateTime.UtcNow, Count = 50 }
            };
            var parameters = new LogStatsTimelineParameters ();
            _mockLogService.Setup(s => s.GetTimelineStatsAsync(parameters))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GetTimelineStats(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedStats, okResult.Value);
        }

        [Fact]
        public async Task GetSources_ReturnsOkResultWithSources()
        {
            // Arrange
            var expectedSources = new List<string> { "Source1", "Source2" };

            _mockLogService.Setup(s => s.GetDistinctSourcesAsync())
                .ReturnsAsync(expectedSources);

            // Act
            var result = await _controller.GetSources();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedSources, okResult.Value);
        }

        [Fact]
        public async Task GetApplications_ReturnsOkResultWithApplications()
        {
            // Arrange
            var expectedApps = new List<string> { "App1", "App2" };

            _mockLogService.Setup(s => s.GetDistinctApplicationsAsync())
                .ReturnsAsync(expectedApps);

            // Act
            var result = await _controller.GetApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedApps, okResult.Value);
        }
    }
}