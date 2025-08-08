using FrontEnd.Blazor.Helpers;
using Microsoft.VisualBasic;
using Shared;
using System;
using System.Net.Http.Json;

namespace FrontEnd.Blazor.Services
{
    public class CustomService : ICustomService
    {
        private readonly GetHttpClient _GetHttpClient;

        public CustomService(GetHttpClient getHttpClient)
        {
            _GetHttpClient = getHttpClient;
        }

        public async Task<IEnumerable<ContactDTO>> GetContactsByNameAsync(string url, string? name)
        {
            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<IEnumerable<ContactDTO>>($"{url}/byName?name={name}");
            return response ?? new List<ContactDTO>();
        }

        public async Task<IEnumerable<ContactDTO>> GetContactByFilterAsync(string url,
             string? email,
             string? company,
             string? firstName,
             string? lastName,
             string? phone)
        {
            var query = new List<string>();
            query.Add($"email={email}");
            query.Add($"company={company}");
            query.Add($"firstName={firstName}");
            query.Add($"lastName={lastName}");
            query.Add($"phone={phone}");
            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<IEnumerable<ContactDTO>>($"{url}/filter{queryString}");
            return response ?? new List<ContactDTO>();
        }

        public async Task<IEnumerable<TxskDTO>> GetTxskByFilterAsync(string url, DateTime? dueDateFrom, DateTime? dueDateTo)
        {
            var query = new List<string>();
            if (dueDateFrom.HasValue)
                query.Add($"dueDateFrom={dueDateFrom.Value.ToString("o")}"); // "o" is the standard format ISO 8601
            if (dueDateTo.HasValue)
                query.Add($"dueDateTo={dueDateTo.Value.ToString("o")}");
            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var httpClient = await _GetHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<IEnumerable<TxskDTO>>($"{url}/filter{queryString}");
            return response ?? new List<TxskDTO>();
        }
    }
}
