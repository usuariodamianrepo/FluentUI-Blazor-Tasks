using FrontEnd.Blazor.Helpers;
using Shared;
using System.Net.Http.Json;

namespace FrontEnd.Blazor.Services
{
    public class GenericService<T> : IGenericService<T>
    {
        private readonly GetHttpClient _GetHttpClient;

        public GenericService(GetHttpClient getHttpClient)
        {
            _GetHttpClient = getHttpClient;
        }

        public async Task<GeneralResponse> CreateAsync(string url, T toCreate)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{url}", toCreate);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<GeneralResponse> DeleteAsync(string url, int id)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.DeleteAsync($"{url}/{id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<IEnumerable<T>>(url);
            return response ?? new List<T>();
        }

        public async Task<T> GetByIdAsync(string url, int id)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<T>($"{url}/{id}");
            return response!;
        }

        public async Task<GeneralResponse> UpdateAsync(string url, int id, T toUpdate)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PutAsJsonAsync($"{url}/{id}", toUpdate);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result!;
        }
    }
}
