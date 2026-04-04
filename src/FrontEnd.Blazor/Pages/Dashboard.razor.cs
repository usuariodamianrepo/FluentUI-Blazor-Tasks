using Microsoft.FluentUI.AspNetCore.Components;

namespace FrontEnd.Blazor.Pages
{
    public partial class Dashboard
    {
        JustifyContent Justification = JustifyContent.FlexStart;
        int Spacing = 3;

        void OnBreakpointEnterHandler(GridItemSize size)
        {
            Console.WriteLine($"Page Size: {size}");
        }
    }
}
