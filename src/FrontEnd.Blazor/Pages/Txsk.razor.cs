using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class Txsk
    {
        string _activeTabId = Constants.TabList;
        string? _messageBar { get; set; }
        bool _messageBarVisible { get; set; }
        MessageIntent _messageIntent { get; set; }
        bool _showProgress { get; set; } = false;
        string? _titleBar { get; set; }
        PaginationState _pagination = new PaginationState() { ItemsPerPage = 10 };

        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }
        [Inject]
        public required ICustomService _CustomService { get; set; }
        [Inject]
        public required IGenericService<TxskDTO> _TxskService { get; set; }
        [Inject]
        public required IGenericService<TxskStatusDTO> _TxskStatusService { get; set; }
        [Inject]
        public required IGenericService<TxskTypeDTO> _TxskTypeService { get; set; }

        DateTime? _DueDateFrom { get; set; } = DateTime.Now.AddMonths(-1);
        DateTime? _DueDateTo { get; set; } = DateTime.Now.AddMonths(1);

        TxskDTO? _Txsk { get; set; }
        IQueryable<TxskDTO>? _Txsks { get; set; }

        IEnumerable<TxskStatusDTO>? _TxskStatuses { get; set; }
        IEnumerable<TxskTypeDTO>? _TxskTypes { get; set; }
        
        List<Option<int>> _StatusOptions = new();
        List<Option<int>> _TypeOptions = new();
        ContactDTO? _ContactSelected = null;

        private string? _TxskStatusIdSelected
        {
            get => _Txsk?.TxskStatusId.ToString();
            set
            {
                if (int.TryParse(value, out int id))
                {
                    _Txsk!.TxskStatusId = id;
                }
            }
        }

        private string? _TxskTypeIdSelected
        {
            get => _Txsk?.TxskTypeId.ToString();
            set
            {
                if (int.TryParse(value, out int id))
                {
                    _Txsk!.TxskTypeId = id;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadRelatedData();
            await LoadSearch();
        }

        private async Task LoadRelatedData()
        {
            try
            {
                if (_ContactService == null) ToastService.ShowError("The Contact Service is not working.");
                if (_TxskTypeService == null) ToastService.ShowError("The TxskType Service is not working.");
                if (_TxskStatusService == null) ToastService.ShowError("The TxskStatus Service is not working.");

                _showProgress = true;
                _TxskTypes = await _TxskTypeService!.GetAllAsync(Constants.TxskTypeApiUrl);
                _TxskStatuses = await _TxskStatusService!.GetAllAsync(Constants.TxskStatusApiUrl);

                if (_TxskTypes != null && _TxskStatuses != null)
                {
                    _TypeOptions = _TxskTypes.Select(t => new Option<int> { Value = t.Id, Text = t.Name ?? string.Empty }).ToList();
                    _StatusOptions = _TxskStatuses.Select(s => new Option<int> { Value = s.Id, Text = s.Name ?? string.Empty }).ToList();
                }
            }
            catch (Exception e)
            {
                MessageBar(MessageIntent.Error, "Error API", e.Message);
            }
            finally
            {
                _showProgress = false;
                StateHasChanged();
            }
        }

        private async Task LoadSearch()
        {
            try
            {
                if (_TxskService == null) ToastService.ShowError("The Service is not working.");
                _showProgress = true;

                var result = await _CustomService!.GetTxskByFilterAsync(Constants.TxskApiUrl, _DueDateFrom, _DueDateTo);
                if (result != null)
                {
                    _Txsks = result.AsQueryable();
                    if (_Txsks.Count() > 100)
                    {
                        MessageBar(MessageIntent.Warning, "Search", "Your search returned more than 100 results. Improve your filter.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBar(MessageIntent.Error, "Error API", e.Message);
            }
            finally
            {
                _showProgress = false;
                StateHasChanged();
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

        private void OnAddClicked()
        {
            _Txsk = new()
            {
                Id = 0,
                TxskTypeId = _TxskTypes?.FirstOrDefault()?.Id ?? 0,
                TxskStatusId = _TxskStatuses?.FirstOrDefault()?.Id ?? 0,
            };
            _ContactSelected = null;
            _activeTabId = Constants.TabContent;
        }

        private void OnCancelClicked()
        {
            _activeTabId = Constants.TabList;
            _Txsk = null;
        }

        private void OnContactChanged(string text)
        {
            _Txsk!.ContactId = _ContactSelected?.Id ?? null;
        }

        private async Task OnContactSearchAsync(OptionsSearchEventArgs<ContactDTO> e)
        {
            if (e.Text.Length < 3)
            {
                _ContactSelected = null;
                return;
            }

            _showProgress = true;
            e.Items = await _CustomService.GetContactsByNameAsync(Constants.ContactApiUrl, e.Text);
            _showProgress = false;
        }

        private async void OnDeleteClicked(TxskDTO itemDTO)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {itemDTO.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (_TxskService != null)
            {
                RefreshData(await _TxskService.DeleteAsync(Constants.TxskApiUrl, itemDTO.Id));
            }
        }

        private async void OnDetailsClicked(TxskDTO itemDTO)
        {
            if (_TxskService == null) return;

            _Txsk = await _TxskService.GetByIdAsync(Constants.TxskApiUrl, itemDTO.Id);
            if (_Txsk != null)
            {
                await DialogService.ShowDialogAsync<TxskDialog>(_Txsk, new DialogParameters()
                {
                    Title = $"Details Txsk Id:{_Txsk.Id}",
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                });
            }
        }

        private async void OnEditClicked(TxskDTO itemDTO)
        {
            if (_TxskService == null)
            {
                ToastService.ShowError("The Tasks Service is not working.");
                return;
            }

            _Txsk = await _TxskService.GetByIdAsync(Constants.TxskApiUrl, itemDTO.Id);

            if (_Txsk != null)
            {
                _activeTabId = Constants.TabContent;
                if (_Txsk.ContactId is null)
                    _ContactSelected = null;
                else
                    _ContactSelected = new ContactDTO { Id = _Txsk.ContactId ?? 0, FirstName = _Txsk.ContactName };

                _TxskTypeIdSelected = _Txsk.TxskTypeId.ToString();
                _TxskStatusIdSelected = _Txsk.TxskStatusId.ToString();
                StateHasChanged();
            }
        }

        private async Task OnSaveContentClicked()
        {
            if (_Txsk!.Id == 0)
            {
                RefreshData(await _TxskService.CreateAsync(Constants.TxskApiUrl, _Txsk));
            }
            else
            {
                RefreshData(await _TxskService.UpdateAsync(Constants.TxskApiUrl, _Txsk.Id, _Txsk));
            }
            _activeTabId = Constants.TabList;
            _Txsk = null;
        }

        private async void OnSearchClicked()
        {
            await LoadSearch();
        }

        private async void RefreshData(GeneralResponse response)
        {
            if (response.Success)
            {
                ToastService.ShowSuccess(response.Message);
                await LoadSearch();
            }
            else
            {
                ToastService.ShowError(response.Message);
            }
        }
    }
}
