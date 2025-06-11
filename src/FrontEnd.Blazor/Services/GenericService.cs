using Shared;
using System.Net.Http.Json;

namespace FrontEnd.Blazor.Services
{
    public class GenericService<T> : IGenericService<T>
    {
        private readonly HttpClient _httpClient;

        public GenericService(HttpClient getHttpClient)
        {
            _httpClient = getHttpClient;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<T>>(url);
            return response ?? new List<T>();
        }

        public async Task<T> GetByIdAsync(string url, int id)
        {
            var response = await _httpClient.GetFromJsonAsync<T>($"{url}/{id}");
            return response!;
        }

        public async Task<GeneralResponse> CreateAsync(string url, T toCreate)
        {
            var response = await _httpClient.PostAsJsonAsync($"{url}", toCreate);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<GeneralResponse> UpdateAsync(string url, int id, T toUpdate)
        {
            var response = await _httpClient.PutAsJsonAsync($"{url}/{id}", toUpdate);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<GeneralResponse> DeleteAsync(string url, int id)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }
    }
}
