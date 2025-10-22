using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LaborControl.Web;
using LaborControl.Web.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient avec l'URL de l'API
// En local, utilise l'API Azure directement
// À modifier pour revenir au localhost:5278 si vous lancez le backend en local
var apiBaseUrl = "https://laborcontrol-api.azurewebsites.net/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Ajouter Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Ajouter les services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
