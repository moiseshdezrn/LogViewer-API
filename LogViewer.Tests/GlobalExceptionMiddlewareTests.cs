using System.Net;
using System.Text.Json;
using LogViewer.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogViewer.Tests
{
    public class GlobalExceptionMiddlewareExceptionHandlingTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareExceptionHandlingTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_WithKeyNotFoundException_Returns404()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new KeyNotFoundException("Resource not found");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(404, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_WithUnauthorizedAccessException_Returns401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new UnauthorizedAccessException("Not authorized");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(401, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithArgumentException_Returns400()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new ArgumentException("Invalid argument");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(400, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithGenericException_Returns500()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new InvalidOperationException("Something went wrong");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(500, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithException_WritesJsonResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            var exception = new Exception("Test exception");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            Assert.Contains("status", responseText);
            Assert.Contains("message", responseText);
        }

        [Fact]
        public async Task InvokeAsync_WithException_IncludesCorrelationIdInResponse()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();
            var context = new DefaultHttpContext();
            context.Items["CorrelationId"] = correlationId;
            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            var exception = new Exception("Test exception");

            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            Assert.Contains(correlationId, responseText);
        }
    }
}
