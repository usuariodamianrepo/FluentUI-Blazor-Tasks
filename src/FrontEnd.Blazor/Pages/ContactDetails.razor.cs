using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared;
using System.Text.Json;

namespace FrontEnd.Blazor.Pages
{
    public partial class ContactDetails
    {
        [Parameter]
        public ContactDTO Details { get; set; } = default!;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        private async Task OnCopyDetailsToClipboard()
        {
            if (Details != null)
            {
                var json = JsonSerializer.Serialize(Details, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);
                ToastService.ShowSuccess($"Contact Id: {Details.Id} copied to clipboard!");
            }
            else
            {
                ToastService.ShowWarning("No contact selected.");
            }
        }
    }
}
