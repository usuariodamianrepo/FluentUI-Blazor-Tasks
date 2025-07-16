using FrontEnd.Blazor.Helpers;
using Shared;
using System.Net.Http.Json;

namespace FrontEnd.Blazor.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly GetHttpClient _GetHttpClient;

        public UserAccountService(GetHttpClient getHttpClient) 
        {
            _GetHttpClient = getHttpClient;
        }

        public async Task<GeneralResponse> CreateAsync(UserRegisterDTO user)
        {
            var httpClient = _GetHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{Constants.AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }

        public async Task<LoginResponse> SignInAsync(UserLoginDTO user)
        {
            var httpClient = _GetHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{Constants.AuthUrl}/login", user);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>()!;
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenDTO token)
        {
            var httpClient = _GetHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{Constants.AuthUrl}/refresh-token", token);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>()!;
        }
    }
}
