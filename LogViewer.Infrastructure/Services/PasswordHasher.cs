using LogViewer.Core.Interfaces;

namespace LogViewer.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string HashPassword(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentException("Password cannot be null or empty", nameof(plainTextPassword));

            return BCrypt.Net.BCrypt.HashPassword(plainTextPassword, WorkFactor);
        }

        public bool VerifyPassword(string plainTextPassword, string hashedPassword)
        {
           
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
