using Shared;

namespace FrontEnd.Blazor.Services
{
    public interface IGenericService<T>
    {
        Task<IEnumerable<T>> GetAllAsync(string url);
        Task<T> GetByIdAsync(string baseUrl, int id);
        Task<GeneralResponse> CreateAsync(string baseUrl, T toCreate);
        Task<GeneralResponse> UpdateAsync(string baseUrl, int id, T toUpdate);
        Task<GeneralResponse> DeleteAsync(string baseUrl, int id);
    }
}
