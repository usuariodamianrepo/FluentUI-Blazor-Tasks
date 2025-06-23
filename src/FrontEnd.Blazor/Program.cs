using FrontEnd.Blazor;
using FrontEnd.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7013") });
builder.Services.AddFluentUIComponents();
builder.Services.AddScoped(typeof(ICustomService), typeof(CustomService));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

await builder.Build().RunAsync();
