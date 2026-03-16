using LogViewer.Core.DTOs;

namespace LogViewer.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
