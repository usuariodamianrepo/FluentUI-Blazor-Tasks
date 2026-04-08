using Microsoft.AspNetCore.Components;
using Shared.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace FrontEnd.Blazor.Pages
{
    public partial class ContactUpsert
    {
        private EditContext _editContext = default!;

        [Parameter]
        public ContactDTO Contact { get; set; } = default!;

        [Parameter]
        public EventCallback OnSave { get; set; }

        protected override void OnInitialized()
        {
            _editContext = new EditContext(Contact);
        }

        private async Task OnSaveClicked()
        {
            if (_editContext.Validate())
            {
                if (OnSave.HasDelegate)
                    // Pass the Contact instance to the parent
                    await OnSave.InvokeAsync(Contact);
            }
        }
    }
}
