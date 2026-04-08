namespace Shared.DTOs
{
    public sealed class UserSessionDTO
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }
    }
}
