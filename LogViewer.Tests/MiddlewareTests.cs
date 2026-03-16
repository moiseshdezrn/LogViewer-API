using LogViewer.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogViewer.Tests
{
    public class CorrelationIdMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<CorrelationIdMiddleware>> _mockLogger;
        private readonly CorrelationIdMiddleware _middleware;

        public CorrelationIdMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<CorrelationIdMiddleware>>();
            _middleware = new CorrelationIdMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_AddsCorrelationIdToContext()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Items.ContainsKey("CorrelationId"));
            var correlationId = context.Items["CorrelationId"];
            Assert.NotNull(correlationId);
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_AddsCorrelationIdToItems()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Items.ContainsKey("CorrelationId"));
            var correlationId = context.Items["CorrelationId"]?.ToString();
            Assert.False(string.IsNullOrEmpty(correlationId));
            Assert.True(Guid.TryParse(correlationId, out _));
        }

        [Fact]
        public async Task InvokeAsync_GeneratesUniqueCorrelationIds()
        {
            // Arrange
            var context1 = new DefaultHttpContext();
            var context2 = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context1);
            await _middleware.InvokeAsync(context2);

            // Assert
            var id1 = context1.Items["CorrelationId"]?.ToString();
            var id2 = context2.Items["CorrelationId"]?.ToString();
            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public async Task InvokeAsync_UsesExistingCorrelationIdFromHeader()
        {
            // Arrange
            var existingId = Guid.NewGuid().ToString();
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Correlation-Id"] = existingId;

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(existingId, context.Items["CorrelationId"]?.ToString());
        }
    }

    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_WithNoException_CallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockNext.Setup(n => n(context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WithException_Returns500()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new Exception("Test exception");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_WithException_LogsError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new Exception("Test exception");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }

    public class RequestLoggingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<RequestLoggingMiddleware>> _mockLogger;
        private readonly RequestLoggingMiddleware _middleware;

        public RequestLoggingMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<RequestLoggingMiddleware>>();
            _middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestInformation()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/logs";
            _mockNext.Setup(n => n(context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GET")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_CallsNextMiddleware()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockNext.Setup(n => n(context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
        }
    }
}
