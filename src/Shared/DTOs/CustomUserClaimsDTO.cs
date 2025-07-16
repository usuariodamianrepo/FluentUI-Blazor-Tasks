namespace Shared
{
    public record CustomUserClaimsDTO(
        string Id = null!,
        string Name = null!,
        string Email = null!,
        string Role = null!);
}
