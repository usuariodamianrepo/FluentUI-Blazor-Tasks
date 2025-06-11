
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class ContactDialog
    {
        private EditContext _editContext= default!;

        [Parameter]
        public ContactDTO Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog Dialog { get; set; } = default!;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(Content);
        }

        private async Task CancelAsync()
        {
            await Dialog.CancelAsync();
        }

        private async Task SaveAsync()
        {
            if (_editContext.Validate())
            {
                await Dialog.CloseAsync(Content);
            }
        }
    }
}