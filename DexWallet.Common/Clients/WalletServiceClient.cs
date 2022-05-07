using System.Net.Http.Headers;
using System.Text.Json;
using DexWallet.Common.Entities.DTOs;
using Microsoft.Net.Http.Headers;

namespace DexWallet.Common.Clients;

public class WalletServiceClient
{
    private readonly HttpClient _httpClient;

    public WalletServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7072");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }

    public async Task<CoreGetWalletByAddressResponseDto> GetWalletByAddressAsync(string authToken, string walletAddress)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/wallets/{walletAddress}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<CoreGetWalletByAddressResponseDto>(stream, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                     ?? new CoreGetWalletByAddressResponseDto(null, false, "deserialization error");

        return result;
    }
}