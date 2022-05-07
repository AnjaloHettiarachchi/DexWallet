namespace DexWallet.Common.Entities.DTOs;

public class CoreGetWalletByAddressResult
{
    public string Address { get; set; } = null!;
    public string Owner { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CryptoType { get; set; } = CryptoExchangeTypes.BTC;
    public decimal CryptoBalance { get; set; }
    public string RegularType { get; set; } = RegularExchangeTypes.USD;
    public decimal RegularBalance { get; set; }
}

public class CoreGetWalletByAddressResponseDto
{
    public CoreGetWalletByAddressResponseDto(CoreGetWalletByAddressResult? result, bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
        Result = result;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public CoreGetWalletByAddressResult? Result { get; set; }
}