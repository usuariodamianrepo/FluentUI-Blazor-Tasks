using Shared;

namespace FrontEnd.Blazor.Services
{
    public interface ICustomService
    {
        Task<IEnumerable<ContactDTO>> GetContactsByNameAsync(string url, string? name);

        Task<IEnumerable<TxskDTO>> GetTxskByFilterAsync(string url, DateTime? dueDateFrom, DateTime? dueDateTo);
    }
}
