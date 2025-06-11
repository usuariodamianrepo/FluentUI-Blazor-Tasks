using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class WeatherDialog
    {
        [Parameter]
        public WeatherForecastDTO Content { get; set; } = default!;

        [CascadingParameter]
        public FluentDialog Dialog { get; set; } = default!;

        private async Task SaveAsync()
        {
            await Dialog.CloseAsync(Content);
        }

        private async Task CancelAsync()
        {
            await Dialog.CancelAsync();
        }
    }
}
