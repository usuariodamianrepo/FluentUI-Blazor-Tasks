---
name: ui-crud-splitter
description: Create a UI CRUD form from a specific Entity, using Blazor classes and FluentUI components. All examples code use T4 text anotation. Copy the code of example and replace the <#=ClassName#> and <#=PluralClassName#> with the name of your class and its plural form.
---

# UI Create

## Set the API URL into the Constants class
At the FrontEnd.Blazor proyect, open the Constants file and add new URL line. 

```csharp   
    public const string <#=ClassName#>ApiUrl = "api/<#=PluralClassName#>";
```  
## Set the Filter endpoint into the Service class
At the FrontEnd.Blazor proyect, open the CustomService file and add new ByFilterAsync line. After that, add the new method into the ICustomService file.

### CustomService.cs
```csharp   
    public async Task<IEnumerable<<#=ClassName#>>> Get<#=ClassName#>ByFilterAsync(string url, string? name)
    {
        var httpClient = await _GetHttpClient.GetPrivateHttpClient();
        var response = await httpClient.GetFromJsonAsync<IEnumerable<<#=ClassName#>DTO>>>($"{url}/filter?name={name}");
        return response ?? new List<<#=ClassName#>DTO>>();
    }
```  
### ICustomService.cs
```csharp   
    Task<IEnumerable<<#=ClassName#>DTO>> Get<#=ClassName#>ByFilterAsync(string url, string? name);
``` 

# Create the Blazor interfaced design file
At the FrontEnd.Blazor proyect, create the Blazor file into the Pages folder with the name of the <#=ClassName#>.razor

```razor
@page "/<#=ClassName#>"
@using FrontEnd.Blazor.Helpers
@using Shared.DTOs;
@using Microsoft.FluentUI.AspNetCore.Components;
@inject IToastService ToastService
@inject IMessageService MessageService

<PageTitle><#=ClassName#></PageTitle>

<h1><#=ClassName#></h1>

<FluentMessageBar Title="@_MessageBarTitle" Visible="@_MessageBarVisible" Intent="@_MessageBarIntent">
    @_MessageBar
</FluentMessageBar>

<FluentKeyCode OnKeyDown="@KeyDownHandler">
    <FluentToolbar>
        <FluentButton Title="Dismiss all filters" IconStart="@(new Icons.Regular.Size20.FilterDismiss())" Appearance="Appearance.Neutral"
                      OnClick="@OnFilterDismissClicked" Disabled=@_DisabledFilterDismiss />
        <FluentButton Title="Add" IconStart="@(new Icons.Regular.Size20.Add())" Appearance="Appearance.Neutral"
                      OnClick="@OnAddClicked" Disabled=@(_<#=PluralClassName#> == null) />
        <FluentTextField @bind-Value=_NameSearch Placeholder="Name" @oninput="OnNameSearchChanged" Maxlength=15 Size=15 AutoComplete="off" Autofocus="true" />
        <FluentButton Title="Search" IconStart="@(new Icons.Regular.Size20.DatabaseSearch())" Appearance="Appearance.Accent"
                      OnClick="@OnSearchClicked" Loading="@_SearchButtonLoading">Search</FluentButton>
        <FluentButton Title="Clear Search" IconStart="@(new Icons.Regular.Size20.SlideEraser())" Appearance="Appearance.Neutral"
                      OnClick="@OnClearSearchClicked"></FluentButton>
        <FluentSwitch CheckedMessage="Collapsed" UncheckedMessage="Split" @bind-Value="_Collapse" />
    </FluentToolbar>
</FluentKeyCode>
<FluentProgress Visible="@_ProgressVisible"></FluentProgress>

<FluentSplitter Collapsed="_Collapse" Panel1Size="60%">
    <Panel1>
        <FluentDataGrid @ref="_Grid" Id="<#=ClassName#>Grid" Items="@_FilteredItems" TGridItem=<#=ClassName#>DTO
                        GridTemplateColumns="auto 1fr 1fr 1fr 1fr auto"
                        ShowHover="true"
                        RowSize="DataGridRowSize.Medium"
                        Pagination="@_Pagination">
<#          
            Type entityType = typeof(Company);
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.Name == "Id")
                {
                  #> <PropertyColumn Title="Id" Property="@(c => c!.Id)" Align="Align.Center" /> <#  
                }
                else if (prop.PropertyType == typeof(int))
                {
                    #> <PropertyColumn Title="<#=prop.PropertyType.Name#>" Property="@(c => c!.<#=prop.PropertyType.Name#>)" Align="Align.Center" /> <# 
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    #> <PropertyColumn Title="<#=prop.PropertyType.Name#>" Property="@(c => c!.<#=prop.PropertyType.Name#>)" Align="Align.Center" Format="dd/MM/yyyy" /> <# 
                }
                else
                {
                    #>
                <PropertyColumn Title="<#=prop.PropertyType.Name#>" Property="@(c => c!.<#=prop.PropertyType.Name#>)" Sortable="true" Filtered="!string.IsNullOrWhiteSpace(_<#=prop.PropertyType.Name#>Filter)" Tooltip="true">
                    <ColumnOptions>
                        <div class="search-box">
                            <FluentSearch Autofocus=true @bind-Value=_<#=prop.PropertyType.Name#>Filter @oninput="Handle<#=prop.PropertyType.Name#>Filter" @onkeydown="HandleCloseFilterAsync" @bind-Value:after="HandleClear" Placeholder="Filter <#=prop.PropertyType.Name#>" Style="width: 100%;" Label="Filter" />
                        </div>
                    </ColumnOptions>
                </PropertyColumn>
                    <# 
                }
            }
#>
            <TemplateColumn Title="Actions" Align="@Align.End">
                <FluentButton Title="Edit" IconEnd="@(new Icons.Regular.Size20.Edit())" OnClick="@(() => OnEditClicked(context))" />
                <FluentButton Title="Details" IconEnd="@(new Icons.Regular.Size20.AppsListDetail())" OnClick="@(() => OnDetailsClicked(context))" />
                <FluentButton Title="Delete" IconEnd="@(new Icons.Regular.Size20.Delete().WithColor(Color.Error))" OnClick="@(() => OnDeleteClicked(context))" />
            </TemplateColumn>
        </FluentDataGrid>
        <FluentPaginator State="@_Pagination" />
    </Panel1>
    <Panel2>
        <FluentTabs ActiveTabId="@_ActiveTabId" ActiveTabIdChanged="@(id => _ActiveTabId = id)">
            <FluentTab Id="@Constants.TabContent">
                <Header>
                    <FluentIcon Value="@(new Icons.Regular.Size20.Edit())" />
                    Content @(_<#=ClassName#>?.Id > 0 ? $" - Id: {_<#=ClassName#>?.Id}" : "")
                </Header>
                <Content>
                    @if (_<#=ClassName#> != null)
                    {
                        <EditForm Model="_<#=ClassName#>" OnValidSubmit="OnSaveContentClicked">
                            <DataAnnotationsValidator />
<#          
            Type entityType = typeof(Company);
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.Name != "Id")
                {
                  if (prop.PropertyType == typeof(int))
                  {
                      #><FluentNumberField Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>.<#=prop.PropertyType.Name#> AutoComplete="off" /><#  
                  }
                  else if (prop.PropertyType == typeof(DateTime))
                  {
                      #><FluentDatePicker Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>.<#=prop.PropertyType.Name#> AutoComplete="off" /><#  
                  }
                  else
                  {
                      #><FluentTextField Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>.<#=prop.PropertyType.Name#> AutoComplete="off" /><# 
                  }
                }
            }
#>
                            <div style="color: var(--error);">
                                <FluentValidationSummary />
                            </div>
                            <br />
                            <FluentToolbar id="toolbar-footer">
                                <FluentButton Type="ButtonType.Submit" Title="Save" IconStart="@(new Icons.Regular.Size20.Save())" Appearance="Appearance.Accent">Save</FluentButton>
                                <FluentButton Title="Cancel" IconStart="@(new Icons.Regular.Size20.ArrowExit())" Appearance="Appearance.Neutral" OnClick="OnCancelContentClicked">Cancel</FluentButton>
                            </FluentToolbar>
                        </EditForm>
                    }
                    else
                    {
                        <p>No <#=ClassName#> selected.</p>
                    }
                </Content>
            </FluentTab>
            <FluentTab Id="@Constants.TabDetails">
                <Header>
                    <FluentIcon Value="@(new Icons.Regular.Size20.AppsListDetail())" />
                    Details @(_<#=ClassName#>Details?.Id > 0 ? $" - Id: {_<#=ClassName#>Details?.Id}" : "")
                </Header>
                <Content>
                    @if (_<#=ClassName#>Details != null)
                    {
<#          
            Type entityType = typeof(Company);
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.Name != "Id")
                {
                  if (prop.PropertyType == typeof(int))
                  {
                      #><FluentTextField Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>Details.<#=prop.PropertyType.Name#> Size=60 ReadOnly /><#  
                  }
                  else if (prop.PropertyType == typeof(DateTime))
                  {
                      #><FluentTextField Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>Details.<#=prop.PropertyType.Name#> Size=30 ReadOnly /><#  
                  }
                  else
                  {
                      #><FluentTextField Label="<#=prop.PropertyType.Name#>" @bind-Value=_<#=ClassName#>Details.<#=prop.PropertyType.Name#> Size=30 ReadOnly /><#  
                  }
                }
            }
#>
                        <div style="color: var(--error);">
                        </div>
                        <br />
                        <FluentToolbar id="toolbar-footer">
                            <FluentButton Title="Copy" IconStart="@(new Icons.Regular.Size20.Copy())" Appearance="Appearance.Neutral" OnClick="OnCopyDetailsToClipboard">Copy</FluentButton>
                            <FluentButton Title="Cancel" IconStart="@(new Icons.Regular.Size20.ArrowExit())" Appearance="Appearance.Neutral" OnClick="OnCancelDetailsClicked">Cancel</FluentButton>
                        </FluentToolbar>
                    }
                    else
                    {
                        <p>No <#=ClassName#> selected.</p>
                    }
                </Content>
            </FluentTab>
        </FluentTabs>
    </Panel2>
</FluentSplitter>

```

# Create the Blazor code-behind file
At the FrontEnd.Blazor proyect, create the Blazor file code-behind into the Pages folder with the name of the <#=ClassName#>.razor.cs

```csharp

using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTOs;
using Shared.Responses;
using System.Text.Json;

namespace FrontEnd.Blazor.Pages
{
    public partial class <#=ClassName#>
    {
        [Inject]
        public required IGenericService<<#=ClassName#>DTO> _<#=ClassName#>Service { get; set; }

        [Inject]
        public required ICustomService _CustomService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        <#=ClassName#>DTO? _<#=ClassName#> { get; set; }
        <#=ClassName#>DTO? _<#=ClassName#>Details { get; set; }
        IQueryable<<#=ClassName#>DTO>? _<#=PluralClassName#> { get; set; }

        #region DataGrid
        string _ActiveTabId = Constants.TabList;
        FluentDataGrid<<#=ClassName#>DTO>? _Grid;
        PaginationState _Pagination = new PaginationState() { ItemsPerPage = Constants.ItemsPerPage };
        #endregion

        #region Search
        string _NameSearch = string.Empty;
        bool _ProgressVisible = false;
        bool _SearchButtonLoading = false;
        #endregion

        #region Filter
        bool _DisabledFilterDismiss = true;
        string _NameFilter = string.Empty;
        private bool _Collapse = true;
        #endregion

        IQueryable<<#=ClassName#>DTO>? _FilteredItems
        {
            get
            {
                var result = _<#=PluralClassName#>;

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

        private void OnAddClicked()
        {
            _<#=ClassName#> = new();
            _Collapse = false;
            _ActiveTabId = Constants.TabContent;
        }

        private void OnCancelContentClicked()
        {
            _ActiveTabId = Constants.TabList;
            _<#=ClassName#> = null;
        }

        private void OnCancelDetailsClicked()
        {
            _ActiveTabId = Constants.TabList;
            _<#=ClassName#>Details = null;
        }

        private async void OnClearSearchClicked()
        {
            _NameSearch = string.Empty;
            await SearchData();
        }

        private void OnCompanySearchChanged(ChangeEventArgs e)
        {
            _CompanySearch = e.Value?.ToString() ?? string.Empty;
        }

        private async Task OnCopyDetailsToClipboard()
        {
            if (_<#=ClassName#>Details != null)
            {
                var json = JsonSerializer.Serialize(_<#=ClassName#>Details, new JsonSerializerOptions { WriteIndented = true });
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);
                ToastService.ShowSuccess($"<#=ClassName#> Id: {_<#=ClassName#>Details.Id} copied to clipboard!");
            }
            else
            {
                ToastService.ShowWarning("No contact selected.");
            }
        }
        
        private async void OnDeleteClicked(<#=ClassName#>DTO itemDTO)
        {
            var dialog = await DialogService.ShowConfirmationAsync($"Are you sure you wanna delete Id: {itemDTO.Id}?", "Yes", "No", "Delete");
            var result = await dialog.Result;

            if (result.Cancelled) return;

            if (_<#=ClassName#>Service != null)
            {
                RefreshData(await _<#=ClassName#>Service.DeleteAsync(Constants.<#=ClassName#>ApiUrl, itemDTO.Id));
            }
        }

        private async void OnDetailsClicked(<#=ClassName#>DTO itemDTO)
        {
            if (_<#=ClassName#>Service == null)
            {
                ToastService.ShowError("The <#=PluralClassName#> Service is not working.");
                return;
            }

            _<#=ClassName#>Details = await _<#=ClassName#>Service.GetByIdAsync(Constants.<#=ClassName#>ApiUrl, itemDTO.Id);

            if (_<#=ClassName#>Details != null)
            {
                _ActiveTabId = Constants.TabDetails;
                _Collapse = false;
                StateHasChanged();
            }
        }

        private async void OnEditClicked(<#=ClassName#>DTO itemDTO)
        {
            if (_<#=ClassName#>Service == null)
            {
                ToastService.ShowError("The <#=ClassName#> Service is not working.");
                return;
            }

            _<#=ClassName#> = await _<#=ClassName#>Service.GetByIdAsync(Constants.<#=ClassName#>ApiUrl, itemDTO.Id);

            if (_<#=ClassName#> != null)
            {
                _ActiveTabId = Constants.TabContent;
                _Collapse = false;
                StateHasChanged();
            }
        }
        
        private void OnFilterDismissClicked()
        {
            _NameFilter = string.Empty;
            OnFilterDismissDisabledChanged();
        }

        private void OnFilterDismissDisabledChanged()
        {
            _DisabledFilterDismiss = string.IsNullOrWhiteSpace(_NameFilter);
        }
        
        private void OnNameSearchChanged(ChangeEventArgs e)
        {
            _NameSearch = e.Value?.ToString() ?? string.Empty;
        }

        private async Task OnSaveContentClicked()
        {
            if (_<#=ClassName#>!.Id == 0)
            {
                RefreshData(await _<#=ClassName#>Service.CreateAsync(Constants.<#=ClassName#>ApiUrl, _<#=ClassName#>));
            }
            else
            {
                RefreshData(await _<#=ClassName#>Service.UpdateAsync(Constants.<#=ClassName#>ApiUrl, _<#=ClassName#>.Id, _<#=ClassName#>));
            }
            _ActiveTabId = Constants.TabList;
            _<#=ClassName#> = null;
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
                
                var result = await _CustomService!.Get<#=ClassName#>ByFilterAsync(Constants.<#=ClassName#>ApiUrl, _NameSearch);
                if (result != null)
                {
                    _<#=PluralClassName#> = result.AsQueryable();
                    if (<#=PluralClassName#>.Count() >= Constants.ItemsMaxNumber)
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
```