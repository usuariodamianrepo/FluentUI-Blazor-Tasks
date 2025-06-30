using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontEnd.Blazor.Services
{
    //TODO: use Microsoft.AspNetCore.Components.WebAssembly.Authentication
    public class AuthService
    {
        private readonly HttpClient _http;
        public string? JwtToken { get; private set; }

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> Login(string username, string password)
        {
            await Task.Delay(1000);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            return username.ToUpper() == password.ToUpper();

            //var response = await _http.PostAsJsonAsync("api/auth/login", new { username, password });

            //if (response.IsSuccessStatusCode)
            //{
            //    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            //    JwtToken = result.GetProperty("token").GetString();

            //    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            //    return true;
            //}

            //return false;
        }
    }
}
