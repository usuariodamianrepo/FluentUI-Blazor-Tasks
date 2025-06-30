namespace FrontEnd.Blazor.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
    }
}
