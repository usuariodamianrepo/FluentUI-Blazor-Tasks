
using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class Contact
    {
        PaginationState _pagination = new PaginationState() { ItemsPerPage = 10 };
        FluentDataGrid<ContactDTO>? grid;

        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }
        ContactDTO? _Contact { get; set; }
        IQueryable<ContactDTO>? _Contacts { get; set; }

        #region Filter
        bool _DisabledFilterDismiss = true;
        string _EmailFilter = string.Empty;
        string _NameFilter = string.Empty;

        IQueryable<ContactDTO>? _FilteredItems
        {
            get
            {
                var result = _Contacts;

                if (result is not null && !string.IsNullOrEmpty(_EmailFilter))
                {
                    result = result.Where(c => c.Email.Contains(_EmailFilter, StringComparison.CurrentCultureIgnoreCase));
                }
                if (result is not null && !string.IsNullOrEmpty(_NameFilter))
                {
                    result = result.Where(c => c.Name.Contains(_NameFilter, StringComparison.CurrentCultureIgnoreCase));
                }

                return result;
            }
        }
        #endregion

        #region MessageBar
        string _messageBar { get; set; } = String.Empty;
        bool _messageBarVisible { get; set; }
        MessageIntent _messageIntent { get; set; }
        string _titleBar { get; set; } = String.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private void HandleClear()
        {
            if (string.IsNullOrWhiteSpace(_EmailFilter))
            {
                _EmailFilter = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(_NameFilter))
            {
                _NameFilter = string.Empty;
            }
        }

        private async Task HandleCloseFilterAsync(KeyboardEventArgs args)
        {
            if (args.Key == Constants.KeyEnter && grid is not null)
            {
                await grid.CloseColumnOptionsAsync();
            }
        }

        private void HandleEmailFilter(ChangeEventArgs args)
        {
            if (args.Value is string value)
            {
                _EmailFilter = value;
                OnFilterDismissDisabledChanged();
            }
        }

        private void HandleNameFilter(ChangeEventArgs args)
        {
            if (args.Value is string value)
            {
                _NameFilter = value;
                OnFilterDismissDisabledChanged();
            }
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

        private void OnFilterDismissClicked()
        {
            _EmailFilter = string.Empty;
            _NameFilter = string.Empty;
            OnFilterDismissDisabledChanged();
        }

        private void OnFilterDismissDisabledChanged()
        {
            _DisabledFilterDismiss = string.IsNullOrWhiteSpace(_EmailFilter) && string.IsNullOrWhiteSpace(_NameFilter);
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
