using System.Net.Http.Headers;
using System.Text.Json;
using DexWallet.Common.Entities.DTOs;
using DexWallet.Common.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DexWallet.Common.Clients;

public class CoreServiceClient
{
    private readonly HttpClient _httpClient;

    public CoreServiceClient(HttpClient httpClient, IOptions<CommonAppSettings> appSettings)
    {
        _httpClient = httpClient;

        if (string.IsNullOrEmpty(appSettings.Value.CoreServiceUrl))
            throw new InvalidOperationException("Invalid Core service URI");
        _httpClient.BaseAddress = new Uri(appSettings.Value.CoreServiceUrl);
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