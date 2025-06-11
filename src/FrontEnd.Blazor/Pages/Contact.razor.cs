
using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class Contact
    {
        private string _messageBar { get; set; } = String.Empty;
        private bool _messageBarVisible { get; set; }
        private MessageIntent _messageIntent { get; set; }
        private string _titleBar { get; set; } = String.Empty;
        private PaginationState _pagination = new PaginationState() { ItemsPerPage = 10 };

        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }

        private ContactDTO? _Contact { get; set; }
        private IQueryable<ContactDTO>? _Contacts { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                if (_ContactService == null) ToastService.ShowError("The Service is not working.");

                var result = await _ContactService!.GetAllAsync(Constants.ContactApiUrl);
                if (result != null)
                {
                    _Contacts = result.AsQueryable();
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
            _Contact = new ContactDTO();

            var dialog = await DialogService.ShowDialogAsync<ContactDialog>(_Contact, new DialogParameters()
            {
                Title = $"Creating Contact",
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                _Contact = (ContactDTO)result.Data;
                RefreshData(await _ContactService.CreateAsync(Constants.ContactApiUrl, _Contact));
            }
        }

        private async void OnDeleteClicked(ContactDTO itemDTO)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {itemDTO.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (_ContactService != null)
            {
                RefreshData(await _ContactService.DeleteAsync(Constants.ContactApiUrl, itemDTO.Id));
            }
        }

        private async void OnDetailsClicked(ContactDTO itemDTO)
        {
            if (_ContactService == null) return;

            _Contact = await _ContactService.GetByIdAsync(Constants.ContactApiUrl, itemDTO.Id);
            if (_Contact != null)
            {
                await DialogService.ShowInfoAsync(
                    $"Email: {_Contact.Email ?? "-"}, " +
                    $"Company: {_Contact.Company ?? "-"}, " +
                    $"LastName: {_Contact.LastName ?? "-"}, " +
                    $"FirstName: {_Contact.FirstName ?? "-"}, " +
                    $"Phone: {_Contact.Phone ?? "-"}, ", $"Details Contact Id: {_Contact.Id}");
            }
        }


        private async void OnEditClicked(ContactDTO itemDTO)
        {
            if (_ContactService == null) return;

            _Contact = await _ContactService.GetByIdAsync(Constants.ContactApiUrl, itemDTO.Id);
            if (_Contact != null)
            {
                var dialog = await DialogService.ShowDialogAsync<ContactDialog>(_Contact, new DialogParameters()
                {
                    Title = $"Updating Contact Id: {_Contact.Id}",
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                });

                var result = await dialog.Result;
                if (!result.Cancelled && result.Data != null)
                {
                    _Contact = (ContactDTO)result.Data;
                    RefreshData(await _ContactService.UpdateAsync(Constants.ContactApiUrl, _Contact.Id, _Contact));
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
