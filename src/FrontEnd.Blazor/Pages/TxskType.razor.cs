using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class TxskType
    {
        [Inject]
        public IGenericService<TxskTypeDTO>? TxskTypeService { get; set; }

        private TxskTypeDTO? _TxskType { get; set; }
        private IQueryable<TxskTypeDTO>? _TxskTypes { get; set; }

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
                if (TxskTypeService == null) ToastService.ShowError("The Service is not working.");

                var result = await TxskTypeService!.GetAllAsync(Constants.TxskTypeApiUrl);
                if (result != null)
                {
                    _TxskTypes = result.AsQueryable();
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
            _TxskType = new TxskTypeDTO();

            var dialog = await DialogService.ShowDialogAsync<TxskTypeDialog>(_TxskType, new DialogParameters()
            {
                Title = $"Creating TxskType",
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                _TxskType = (TxskTypeDTO)result.Data;
                RefreshData(await TxskTypeService!.CreateAsync(Constants.TxskTypeApiUrl, _TxskType));
            }
        }

        private async void OnDeleteClicked(TxskTypeDTO itemDTO)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {itemDTO.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (TxskTypeService != null)
            {
                RefreshData(await TxskTypeService.DeleteAsync(Constants.TxskTypeApiUrl, itemDTO.Id));
            }
        }

        private async void OnDetailsClicked(TxskTypeDTO itemDTO)
        {
            if (TxskTypeService == null) return;

            _TxskType = await TxskTypeService.GetByIdAsync(Constants.TxskTypeApiUrl, itemDTO.Id);
            if (_TxskType != null)
            {
                await DialogService.ShowInfoAsync($"Name: {_TxskType.Name ?? "-"}", $"Details TxskType Id:{_TxskType.Id}");
            }
        }

        private async void OnEditClicked(TxskTypeDTO itemDTO)
        {
            if (TxskTypeService == null) return;

            _TxskType = await TxskTypeService.GetByIdAsync(Constants.TxskTypeApiUrl, itemDTO.Id);
            if (_TxskType != null)
            {
                var dialog = await DialogService.ShowDialogAsync<TxskTypeDialog>(_TxskType, new DialogParameters()
                {
                    Title = $"Updating TxskType Id: {_TxskType.Id}",
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                });

                var result = await dialog.Result;
                if (!result.Cancelled && result.Data != null)
                {
                    _TxskType = (TxskTypeDTO)result.Data;
                    RefreshData(await TxskTypeService.UpdateAsync(Constants.TxskTypeApiUrl, _TxskType.Id, _TxskType));
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