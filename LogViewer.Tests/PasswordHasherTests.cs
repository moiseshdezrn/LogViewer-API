using LogViewer.Infrastructure.Services;

namespace LogViewer.Tests
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsHashedString()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotEqual(password, hash);
            Assert.StartsWith("$2a$", hash);
        }

        [Fact]
        public void HashPassword_WithSamePassword_GeneratesDifferentHashes()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash1 = _passwordHasher.HashPassword(password);
            var hash2 = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "Password123!";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var wrongPassword = "WrongPassword";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword("", hash);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("short")]
        [InlineData("verylongpasswordwithmanychars123456789")]
        [InlineData("P@ssw0rd!")]
        [InlineData("123456")]
        public void HashPassword_WithVariousPasswords_GeneratesValidHash(string password)
        {
            // Act
            var hash = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.True(_passwordHasher.VerifyPassword(password, hash));
        }
    }
}
