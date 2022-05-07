using System.Net.Http.Headers;
using System.Text.Json;
using DexWallet.Common.Models.DTOs;
using Microsoft.Net.Http.Headers;

namespace DexWallet.Common.Clients;

public class IdentityServiceClient
{
    private readonly HttpClient _httpClient;

    public IdentityServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7147");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }

    public async Task<IdentityValidateResponseDto> ValidateTokenAsync(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/identity/validate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<IdentityValidateResponseDto>(stream, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                     ?? new IdentityValidateResponseDto(null, false, "deserialization error");

        return result;
    }
}