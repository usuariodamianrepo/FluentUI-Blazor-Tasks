using Shared;

namespace FrontEnd.Blazor.Services
{
    public interface IGenericService<T>
    {
        Task<GeneralResponse> CreateAsync(string baseUrl, T toCreate);

        Task<GeneralResponse> DeleteAsync(string baseUrl, int id);

        Task<IEnumerable<T>> GetAllAsync(string url);

        Task<T> GetByIdAsync(string baseUrl, int id);

        Task<GeneralResponse> UpdateAsync(string baseUrl, int id, T toUpdate);
    }
}
