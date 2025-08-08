using Shared;

namespace FrontEnd.Blazor.Services
{
    public interface ICustomService
    {
        Task<IEnumerable<ContactDTO>> GetContactByFilterAsync(string url, string? email, string? company, string? firstName, string? lastName, string? phone);
        Task<IEnumerable<ContactDTO>> GetContactsByNameAsync(string url, string? name);

        Task<IEnumerable<TxskDTO>> GetTxskByFilterAsync(string url, DateTime? dueDateFrom, DateTime? dueDateTo);
    }
}
