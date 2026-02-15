
using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared;
using System.Text.Json;
using static FrontEnd.Blazor.Helpers.Emuns;

namespace FrontEnd.Blazor.Pages
{
    public partial class ContactDinamicTab
    {
        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }

        [Inject]
        public required ICustomService _CustomService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        ContactDTO? _Contact { get; set; }
        ContactDTO? _ContactDetails { get; set; }
        IQueryable<ContactDTO>? _Contacts { get; set; }

        #region DataGrid
        FluentDataGrid<ContactDTO>? _Grid;
        PaginationState _Pagination = new PaginationState() { ItemsPerPage = Constants.ItemsPerPage };
        private record TabItem(string Id, ContactDTO Contact, OperationTypes Operation);
        private List<TabItem> _tabs = new();
        string _ActiveTabId = $"{Constants.TabList}-1";
        int _nextTabIndex = 1;
        #endregion

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
        string _MessageBar { get; set; } = String.Empty;
        MessageIntent _MessageBarIntent { get; set; }
        string _MessageBarTitle { get; set; } = String.Empty;
        bool _MessageBarVisible { get; set; }
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
            if (e.KeyCode == Constants.KeyCodeEnter)
            {
                StateHasChanged();
                await SearchData();
            }
        }

        private void MessageBar(MessageIntent intent, string title, string message)
        {
            _MessageBarIntent = intent;
            _MessageBarTitle = title;
            _MessageBar = message;
            _MessageBarVisible = true;
            StateHasChanged();
        }

        private async Task OnAddClicked()
        {
            if (_tabs.Count > 9)
                return; // Limit to 10 tabs for demo purposes

            var addTab = _tabs.FirstOrDefault(t => t.Operation == OperationTypes.Add);
            if (addTab != null)
            {
                _ActiveTabId = addTab.Id;
                return;
            }

            _nextTabIndex++;
            var tabId = $"{Constants.TabList}-{_nextTabIndex}";
            _Contact = new ContactDTO();
            _tabs.Add(new TabItem(tabId, _Contact, OperationTypes.Add));
            await Task.Delay(1);
            _ActiveTabId = tabId;
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
            if (_ContactService == null)
            {
                ToastService.ShowError("The Contacts Service is not working.");
                return;
            }

            if (_tabs.Count > 9) // Limit to 10 tabs for demo purposes
            {
                ToastService.ShowError("You can open only 10 taps.");
                return;
            }

            var contact = await _ContactService.GetByIdAsync(Constants.ContactApiUrl, itemDTO.Id);
            if (contact == null)
            {
                ToastService.ShowError($"The Contact Id: {itemDTO.Id} was deleted.");
                return;
            }

            _nextTabIndex++;
            var tabId = $"{Constants.TabList}-{_nextTabIndex}";
            _tabs.Add(new TabItem(tabId, contact, OperationTypes.Details));
            await Task.Delay(100);
            _ActiveTabId = tabId;
            Console.WriteLine($"{_ActiveTabId} for Contact Id: {itemDTO.Id}");
            StateHasChanged();
        }

        private void OnRemoveTab(string id)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == id);
            if (tab is null)
                return;

            _tabs.Remove(tab);
            _ActiveTabId = $"{Constants.TabList}-1";
        }

        private async void OnEditClicked(ContactDTO itemDTO)
        {
            if (_ContactService == null)
            {
                ToastService.ShowError("The Contacts Service is not working.");
                return;
            }

            if (_tabs.Count > 9) // Limit to 10 tabs for demo purposes
            {
                ToastService.ShowError("You can open only 10 taps.");
                return;
            }

           _Contact = await _ContactService.GetByIdAsync(Constants.ContactApiUrl, itemDTO.Id);
            if (_Contact == null)
            {
                ToastService.ShowError($"The Contact Id: {itemDTO.Id} was deleted.");
                return;
            }

            _nextTabIndex++;
            var tabId = $"{Constants.TabList}-{_nextTabIndex}";
            _tabs.Add(new TabItem(tabId, _Contact, OperationTypes.Edit));
            await Task.Delay(100);
            _ActiveTabId = tabId;
            Console.WriteLine($"{_ActiveTabId} for Contact Id: {itemDTO.Id}");
            StateHasChanged();
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

        private async Task OnSaveContentClicked()
        {
            if (_Contact!.Id == 0)
            {
                RefreshData(await _ContactService.CreateAsync(Constants.ContactApiUrl, _Contact));
            }
            else
            {
                RefreshData(await _ContactService.UpdateAsync(Constants.ContactApiUrl, _Contact.Id, _Contact));
            }
            // Remove the tab that contained this contact (match by reference or id)
            var tab = _tabs.FirstOrDefault(t => ReferenceEquals(t.Contact, _Contact) || t.Contact.Id == _Contact.Id);
            if (tab != null)
            {
                _tabs.Remove(tab);
            }

            _ActiveTabId = $"{Constants.TabList}-1";
            _Contact = null;
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
                        _MessageBarVisible = false;
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
