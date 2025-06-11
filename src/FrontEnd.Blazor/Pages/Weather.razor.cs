using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class Weather
    {
        [Inject]
        public required IGenericService<WeatherForecastDTO> WeatherService { get; set; }

        private GeneralResponse? GeneralResponse { get; set; }
        private string MessageBar { get; set; } = String.Empty;
        private bool MessageBarVisible { get; set; }
        private MessageIntent MessageIntent { get; set; }

        private WeatherForecastDTO? SelectedWeather { get; set; }
        private IQueryable<WeatherForecastDTO>? Weathers { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                if (WeatherService != null)
                {
                    var result = await WeatherService!.GetAllAsync(Constants.WeatherForecastApiUrl);
                    if (result != null)
                    {
                        Weathers = result.AsQueryable();
                        StateHasChanged();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBar = e.Message;
                MessageIntent = MessageIntent.Error;
                MessageBarVisible = true;
            }
        }

        private async void OnAddClicked()
        {
            SelectedWeather = new WeatherForecastDTO
            {
                Id = 0,
                Date = DateTime.Now,
                TemperatureC = 0,
                Summary = string.Empty
            };

            var dialog = await DialogService.ShowDialogAsync<WeatherDialog>(SelectedWeather, new DialogParameters()
            {
                Title = $"Creating Weather",
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                SelectedWeather = (WeatherForecastDTO)result.Data;
                GeneralResponse = await WeatherService.CreateAsync(Constants.WeatherForecastApiUrl, SelectedWeather);
                if (GeneralResponse.Success)
                    ToastService.ShowSuccess(GeneralResponse.Message);
                else
                    ToastService.ShowError(GeneralResponse.Message);
            }
        }

        private async void OnDeleteClicked(WeatherForecastDTO item)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {item.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (WeatherService != null)
            {
                GeneralResponse = await WeatherService.DeleteAsync(Constants.WeatherForecastApiUrl, item.Id);
                if (GeneralResponse.Success)
                    ToastService.ShowSuccess(GeneralResponse.Message);
                else
                    ToastService.ShowError(GeneralResponse.Message);
            }
        }

        private async void OnDetailsClicked(WeatherForecastDTO item)
        {
            if (WeatherService == null) return;

            SelectedWeather = await WeatherService.GetByIdAsync(Constants.WeatherForecastApiUrl, item.Id);
            if (SelectedWeather != null)
            {
                await DialogService.ShowInfoAsync($"Date: {SelectedWeather.Date?.ToShortDateString()}, Temperature: {SelectedWeather.TemperatureC}°C, Summary: {SelectedWeather.Summary}", "Weather Details");
            }
        }


        private async void OnEditClicked(WeatherForecastDTO item)
        {
            if (WeatherService == null) return;

            SelectedWeather = await WeatherService.GetByIdAsync(Constants.WeatherForecastApiUrl, item.Id);
            if (SelectedWeather != null)
            {
                var data = SelectedWeather;

                var dialog = await DialogService.ShowDialogAsync<WeatherDialog>(data, new DialogParameters()
                {
                    Title = $"Updating Weather Id: {SelectedWeather.Id}",
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                });

                var result = await dialog.Result;
                if (!result.Cancelled && result.Data != null)
                {
                    SelectedWeather = (WeatherForecastDTO)result.Data;
                    GeneralResponse = await WeatherService.UpdateAsync(Constants.WeatherForecastApiUrl, SelectedWeather.Id, SelectedWeather);
                    if (GeneralResponse.Success)
                        ToastService.ShowSuccess(GeneralResponse.Message);
                    else
                        ToastService.ShowError(GeneralResponse.Message);
                }
            }
        }
    }
}
