using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class TxskDialog
    {
        private EditContext _detailContext = default!;

        [Parameter]
        public TxskDTO Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog Dialog { get; set; } = default!;

        protected override void OnInitialized()
        {
            _detailContext = new EditContext(Content);
        }

        private async Task AccentAsync()
        {
            await Dialog.CancelAsync();
        }
    }
}