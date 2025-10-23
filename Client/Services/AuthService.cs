using VitrineFr.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace VitrineFr.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<(bool Success, bool RequiresPasswordChange, string? UserId)> LoginAsync(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (loginResponse != null)
            {
                await _localStorage.SetItemAsync("authToken", loginResponse.Token);
                await _localStorage.SetItemAsync("userEmail", loginResponse.User.Email ?? "");
                await _localStorage.SetItemAsync("userRole", loginResponse.User.Role);
                await _localStorage.SetItemAsync("customerId", loginResponse.User.CustomerId.ToString());
                await _localStorage.SetItemAsync("userId", loginResponse.User.Id.ToString());

                return (true, loginResponse.RequiresPasswordChange, loginResponse.User.Id.ToString());
            }
        }
        return (false, false, null);
    }

    private class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public bool RequiresPasswordChange { get; set; }
        public UserDto User { get; set; } = new();
    }

    private class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string Role { get; set; } = "";
        public Guid CustomerId { get; set; }
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterProfessional(
        string email,
        string password,
        string companyName,
        string siret,
        string address,
        string postalCode,
        string city,
        string? website,
        string firstName,
        string lastName,
        string phone,
        string jobTitle)
    {
        var request = new
        {
            Email = email,
            Password = password,
            CompanyName = companyName,
            Siret = siret,
            Address = address,
            PostalCode = postalCode,
            City = city,
            Website = website,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            JobTitle = jobTitle
        };

        var response = await _httpClient.PostAsJsonAsync("api/Auth/register-professional", request);

        if (response.IsSuccessStatusCode)
        {
            // Authentifier automatiquement après inscription
            var (loginSuccess, _, _) = await LoginAsync(email, password);
            return (loginSuccess, null);
        }

        // Lire le message d'erreur de l'API
        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<RegisterErrorResponse>();
            return (false, errorResponse?.Message ?? "Échec de l'inscription");
        }
        catch
        {
            return (false, "Échec de l'inscription. Vérifiez vos informations.");
        }
    }

    private class RegisterErrorResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userEmail");
        await _localStorage.RemoveItemAsync("userRole");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public async Task<string?> GetUserEmailAsync()
    {
        return await _localStorage.GetItemAsync<string>("userEmail");
    }

    public async Task<string?> GetUserRoleAsync()
    {
        return await _localStorage.GetItemAsync<string>("userRole");
    }
}
