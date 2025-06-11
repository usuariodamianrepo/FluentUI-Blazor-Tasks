
using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class TxskStatus
    {
        [Inject]
        public required IGenericService<TxskStatusDTO> TxskStatusService { get; set; }

        private TxskStatusDTO? _TxskStatus { get; set; }
        private IQueryable<TxskStatusDTO>? _TxskStatuses { get; set; }
        private string _messageBar { get; set; } = String.Empty;
        private bool _messageBarVisible { get; set; }
        private MessageIntent _messageIntent { get; set; }
        private string _titleBar { get; set; } = String.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                if (TxskStatusService == null) ToastService.ShowError("The Service is not working.");

                var result = await TxskStatusService!.GetAllAsync(Constants.TxskStatusApiUrl);
                if (result != null)
                {
                    _TxskStatuses = result.AsQueryable();
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                MessageBar(MessageIntent.Error, "Error API", e.Message);
            }
        }

        private void MessageBar(MessageIntent intent, string title, string message)
        {
            _messageIntent = intent;
            _titleBar = title;
            _messageBar = message;
            _messageBarVisible = true;
            StateHasChanged();
        }

        private async void OnAddClicked()
        {
            _TxskStatus = new TxskStatusDTO();

            var dialog = await DialogService.ShowDialogAsync<TxskStatusDialog>(_TxskStatus, new DialogParameters()
            {
                Title = $"Creating TxskStatus",
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                _TxskStatus = (TxskStatusDTO)result.Data;
                RefreshData(await TxskStatusService.CreateAsync(Constants.TxskStatusApiUrl, _TxskStatus));
            }
        }

        private async void OnDeleteClicked(TxskStatusDTO itemDTO)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {itemDTO.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (TxskStatusService != null)
            {
                RefreshData(await TxskStatusService.DeleteAsync(Constants.TxskStatusApiUrl, itemDTO.Id));
            }
        }

        private async void OnDetailsClicked(TxskStatusDTO itemDTO)
        {
            if (TxskStatusService == null) return;

            _TxskStatus = await TxskStatusService.GetByIdAsync(Constants.TxskStatusApiUrl, itemDTO.Id);
            if (_TxskStatus != null)
            {
                await DialogService.ShowInfoAsync(      
                    $"Name: {_TxskStatus.Name ?? "-"}", $"Details TxskStatus Id:{_TxskStatus.Id}");
            }
        }


        private async void OnEditClicked(TxskStatusDTO itemDTO)
        {
            if (TxskStatusService == null) return;

            _TxskStatus = await TxskStatusService.GetByIdAsync(Constants.TxskStatusApiUrl, itemDTO.Id);
            if (_TxskStatus != null)
            {
                var dialog = await DialogService.ShowDialogAsync<TxskStatusDialog>(_TxskStatus, new DialogParameters()
                {
                    Title = $"Updating TxskStatus Id: {_TxskStatus.Id}",
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                });

                var result = await dialog.Result;
                if (!result.Cancelled && result.Data != null)
                {
                    _TxskStatus = (TxskStatusDTO)result.Data;
                    RefreshData(await TxskStatusService.UpdateAsync(Constants.TxskStatusApiUrl, _TxskStatus.Id, _TxskStatus));
                }
            }
        }

        private async void RefreshData(GeneralResponse response)
        {
            if (response.Success)
            {
                ToastService.ShowSuccess(response.Message);
                await LoadData();
            }
            else
            {
                ToastService.ShowError(response.Message);
            }
        }
    }
}
