using System.ComponentModel.DataAnnotations;

namespace LogViewer.Core.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public override string ToString() => $"LoginRequest(Email={Email})";
    }
}
