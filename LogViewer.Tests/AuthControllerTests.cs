using LogViewer.API.Controllers;
using LogViewer.Core.DTOs;
using LogViewer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogViewer.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var expectedResponse = new LoginResponse
            {
                Token = "test-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Username = "testuser",
                Email = request.Email
            };

            _mockAuthService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockAuthService.Verify(s => s.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var value = unauthorizedResult.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest { Email = "", Password = "" };
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_LogsWarningOnFailedAttempt()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            await _controller.Login(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed login attempt")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Login_LogsInformationOnSuccess()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var response = new LoginResponse
            {
                Token = "token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Username = "testuser",
                Email = request.Email
            };

            _mockAuthService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(response);

            // Act
            await _controller.Login(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("logged in successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
