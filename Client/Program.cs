using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VitrineFr;
using VitrineFr.Services;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient avec l'URL de l'API
// En local ET en production : utilise le backend Azure
// avec la base de données PostgreSQL Azure
var apiBaseUrl = "https://laborcontrol-api.azurewebsites.net/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Ajouter Blazored SessionStorage (déconnexion automatique à la fermeture du navigateur)
builder.Services.AddBlazoredSessionStorage();

// Ajouter les services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
