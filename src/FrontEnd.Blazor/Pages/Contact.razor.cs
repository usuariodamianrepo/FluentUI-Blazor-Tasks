
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
        FluentDataGrid<ContactDTO>? _Grid;
        PaginationState _Pagination = new PaginationState() { ItemsPerPage = Constants.ItemsPerPage};
        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }
        [Inject]
        public required ICustomService _CustomService { get; set; }

        ContactDTO? _Contact { get; set; }
        IQueryable<ContactDTO>? _Contacts { get; set; }

        #region Search
        string _CompanySearch = string.Empty;
        string _EmailSearch = string.Empty;
        string _FirstNameSearch = string.Empty;
        string _LastNameSearch = string.Empty;
        string _PhoneSearch = string.Empty;
        bool _ProgressVisible = false;
        bool _SearchButtonLoading = false;
        #endregion

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
            await SearchData();
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
            if (args.Key == Constants.KeyEnter && _Grid is not null)
            {
                await _Grid.CloseColumnOptionsAsync();
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

        private async void KeyDownHandler(FluentKeyCodeEventArgs e)
        {
            if(e.KeyCode == Constants.KeyCodeEnter)
            {
                StateHasChanged();
                await SearchData();
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

        private async void OnClearSearchClicked()
        {
            _EmailSearch = _PhoneSearch = _CompanySearch = _FirstNameSearch = _LastNameSearch = string.Empty;
            await SearchData();
        }

        private void OnCompanySearchChanged(ChangeEventArgs e)
        {
            _CompanySearch = e.Value?.ToString() ?? string.Empty;
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

        private void OnEmailSearchChanged(ChangeEventArgs e)
        {
            _EmailSearch = e.Value?.ToString() ?? string.Empty;
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

        private void OnFirstNameSearchChanged(ChangeEventArgs e)
        {
            _FirstNameSearch = e.Value?.ToString() ?? string.Empty;
        }

        private void OnLastNameSearchChanged(ChangeEventArgs e)
        {
            _LastNameSearch = e.Value?.ToString() ?? string.Empty;
        }
        
        private void OnPhoneSearchChanged(ChangeEventArgs e)
        {
            _PhoneSearch = e.Value?.ToString() ?? string.Empty;
        }

        private async void OnSearchClicked()
        {
            await SearchData();
        }

        private async void RefreshData(GeneralResponse response)
        {
            if (response.Success)
            {
                ToastService.ShowSuccess(response.Message);
                await SearchData();
            }
            else
            {
                ToastService.ShowError(response.Message);
            }
        }

        private async Task SearchData()
        {
            try
            {
                _ProgressVisible = _SearchButtonLoading = true;

                if (_CustomService == null) ToastService.ShowError("The Service is not working.");
                
                var result = await _CustomService!.GetContactByFilterAsync(Constants.ContactApiUrl, _EmailSearch, _CompanySearch, _FirstNameSearch, _LastNameSearch, _PhoneSearch);
                if (result != null)
                {
                    _Contacts = result.AsQueryable();
                    if (_Contacts.Count() >= Constants.ItemsMaxNumber)
                    {
                        MessageBar(MessageIntent.Warning, "Search", $"Your search returned more than {Constants.ItemsMaxNumber} results. Improve your filter.");
                    }
                    else
                    {
                        _messageBarVisible = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBar(MessageIntent.Error, "Error API", e.Message);
            }
            finally
            {
                _ProgressVisible = _SearchButtonLoading = false;
                StateHasChanged();
            }
        }
    }
}
