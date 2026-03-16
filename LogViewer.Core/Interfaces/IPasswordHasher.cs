namespace LogViewer.Core.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string plainTextPassword);
        bool VerifyPassword(string plainTextPassword, string hashedPassword);
    }
}
