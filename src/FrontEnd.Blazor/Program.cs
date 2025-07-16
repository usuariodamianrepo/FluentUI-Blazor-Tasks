using Blazored.LocalStorage;
using FrontEnd.Blazor;
using FrontEnd.Blazor.Helpers;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7013") });
builder.Services.AddTransient<CustomHtppHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7013");
}).AddHttpMessageHandler<CustomHtppHandler>();


builder.Services.AddFluentUIComponents();

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped(typeof(IUserAccountService), typeof(UserAccountService));
builder.Services.AddScoped(typeof(ICustomService), typeof(CustomService));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

await builder.Build().RunAsync();
