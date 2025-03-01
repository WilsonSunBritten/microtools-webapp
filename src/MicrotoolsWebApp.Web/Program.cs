using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MicrotoolsWebApp.Web;
using MicrotoolsWebApp.Web.Services;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load configuration
var apiUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? builder.HostEnvironment.BaseAddress;

// Configure HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddScoped<MicrotoolsService>();

await builder.Build().RunAsync();
