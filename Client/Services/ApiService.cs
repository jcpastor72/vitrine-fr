using VitrineFr.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace VitrineFr.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        return _httpClient;
    }

    // Clients
    public async Task<List<Client>?> GetClientsAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<List<Client>>("api/Customers");
    }

    public async Task<Client?> GetClientAsync(Guid id)
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<Client>($"api/Customers/{id}");
    }

    public async Task<bool> CreateClientAsync(Client newClient)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("api/Customers", newClient);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateClientAsync(Guid id, Client updatedClient)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync($"api/Customers/{id}", updatedClient);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteClientAsync(Guid id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"api/Customers/{id}");
        return response.IsSuccessStatusCode;
    }

    // RFID Chips
    public async Task<List<RfidChipResponse>?> GetRfidChipsAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<List<RfidChipResponse>>("api/RfidChips");
    }

    public async Task<RfidChip?> GetRfidChipAsync(Guid id)
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<RfidChip>($"api/RfidChips/{id}");
    }

    public async Task<bool> CreateRfidChipAsync(RfidChip newChip)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("api/RfidChips", newChip);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRfidChipAsync(Guid id, RfidChip updatedChip)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync($"api/RfidChips/{id}", updatedChip);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRfidChipAsync(Guid id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"api/RfidChips/{id}");
        return response.IsSuccessStatusCode;
    }
}
