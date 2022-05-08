using System.Net.Http.Headers;
using System.Text.Json;
using DexWallet.Common.Helpers;
using DexWallet.Common.Models.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DexWallet.Common.Clients;

public class IdentityServiceClient
{
    private readonly HttpClient _httpClient;

    public IdentityServiceClient(HttpClient httpClient, IOptions<CommonAppSettings> appSettings)
    {
        _httpClient = httpClient;

        if (string.IsNullOrEmpty(appSettings.Value.IdentityServiceUrl))
            throw new InvalidOperationException("Invalid Identity service URI");
        _httpClient.BaseAddress = new Uri(appSettings.Value.IdentityServiceUrl);
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