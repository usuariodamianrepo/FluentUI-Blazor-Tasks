using Shared;

namespace FrontEnd.Blazor.Services
{
    public interface IUserAccountService
    {
        Task<GeneralResponse> CreateAsync(UserRegisterDTO user);
        Task<LoginResponse> SignInAsync(UserLoginDTO user);
        Task<LoginResponse> RefreshToken(RefreshTokenDTO token);
    }
}
