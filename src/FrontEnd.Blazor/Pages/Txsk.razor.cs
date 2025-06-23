using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared;

namespace FrontEnd.Blazor.Pages
{
    public partial class Txsk
    {
        private string _activeTabId = Constants.TabList;
        private PaginationState _pagination = new PaginationState() { ItemsPerPage = 10 };
        private string _messageBar { get; set; } = String.Empty;
        private bool _messageBarVisible { get; set; }
        private MessageIntent _messageIntent { get; set; }
        private string _titleBar { get; set; } = String.Empty;
        private bool _showProgress { get; set; } = false;

        [Inject]
        public required IGenericService<ContactDTO> _ContactService { get; set; }
        [Inject]
        public required IGenericService<TxskDTO> _TxskService { get; set; }
        [Inject]
        public required IGenericService<TxskStatusDTO> _TxskStatusService { get; set; }
        [Inject]
        public required IGenericService<TxskTypeDTO> _TxskTypeService { get; set; }

        [Inject]
        public required ICustomService _CustomService { get; set; }

        private TxskDTO? _Txsk { get; set; }
        private IQueryable<TxskDTO>? _Txsks { get; set; }

        private List<Option<int?>> _ContactOptions = new();
        private List<Option<int>> _StatusOptions = new();
        private List<Option<int>> _TypeOptions = new();

        DateTime? _DueDateFrom { get; set; } = DateTime.Now;
        DateTime? _DueDateTo { get; set; } = DateTime.Now.AddDays(1);

        private string? _ContactIdSelected
        {
            get => _Txsk?.ContactId.ToString();
            set
            {
                if (int.TryParse(value, out int id))
                {
                    _Txsk!.ContactId = id;
                }
                else if (value == string.Empty)
                {
                    _Txsk!.ContactId = null;
                }
            }
        }
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

        private async Task LoadRelatedData()
        {
            try
            {
                if (_ContactService == null) ToastService.ShowError("The Contact Service is not working.");
                if (_TxskTypeService == null) ToastService.ShowError("The TxskType Service is not working.");
                if (_TxskStatusService == null) ToastService.ShowError("The TxskStatus Service is not working.");

                var contacts = await _ContactService!.GetAllAsync(Constants.ContactApiUrl);
                var txskTypes = await _TxskTypeService!.GetAllAsync(Constants.TxskTypeApiUrl);
                var txskStatus = await _TxskStatusService!.GetAllAsync(Constants.TxskStatusApiUrl);

                if (contacts != null && txskTypes != null && txskStatus != null)
                {
                    _ContactOptions = contacts.Select(c => new Option<int?> { Value = c.Id, Text = c.Name ?? string.Empty }).ToList();
                    _ContactOptions.Insert(0, new Option<int?> { Value = null, Text = Constants.OptionEmpty });
                    _TypeOptions = txskTypes.Select(t => new Option<int> { Value = t.Id, Text = t.Name ?? string.Empty }).ToList();
                    _StatusOptions = txskStatus.Select(s => new Option<int> { Value = s.Id, Text = s.Name ?? string.Empty }).ToList();
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

        private void OnAddClicked()
        {
            _activeTabId = Constants.TabContent;
            TxskClean();
        }

        private void OnCancelClicked()
        {
            _activeTabId = Constants.TabList;
            TxskClean();
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
            if (_TxskService == null) return;
            _Txsk = await _TxskService.GetByIdAsync(Constants.TxskApiUrl, itemDTO.Id);

            if (_Txsk != null)
            {
                _activeTabId = Constants.TabContent;
                _ContactIdSelected = _Txsk.ContactId.ToString();
                _TxskTypeIdSelected = _Txsk.TxskTypeId.ToString();
                _TxskStatusIdSelected = _Txsk.TxskStatusId.ToString();
                StateHasChanged();
            }
        }

        private async Task OnSaveDetailClicked()
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

        private void TxskClean()
        {
            _Txsk = null;
        }
    }
}
