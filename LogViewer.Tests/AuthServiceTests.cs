using LogViewer.Core.DTOs;
using LogViewer.Core.Entities;
using LogViewer.Core.Interfaces;
using LogViewer.Infrastructure.Repositories.Interfaces;
using LogViewer.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LogViewer.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["Jwt:Secret"])
                .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"])
                .Returns("LogViewerAPI");
            _mockConfiguration.Setup(c => c["Jwt:Audience"])
                .Returns("LogViewerClient");
            _mockConfiguration.Setup(c => c["Jwt:ExpirationMinutes"])
                .Returns("60");

            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = request.Email,
                Password = "hashed-password"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.Password))
                .Returns(true);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
            _mockPasswordHasher.Verify(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = request.Email,
                Password = "hashed-password"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.Password))
                .Returns(false);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_GeneratesValidJwtToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = request.Email,
                Password = "hashed-password"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.Password))
                .Returns(true);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Contains(".", result.Token);
        }

        [Fact]
        public void LoginAsync_ThrowsExceptionWhenJwtSecretNotConfigured()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Secret"]).Returns((string?)null);

            var service = new AuthService(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object,
                mockConfig.Object);

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = request.Email,
                Password = "hashed-password"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.Password))
                .Returns(true);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.LoginAsync(request));
        }
    }
}
