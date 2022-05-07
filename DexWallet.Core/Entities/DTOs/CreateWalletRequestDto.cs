using DexWallet.Common.Entities;

namespace DexWallet.Core.Entities.DTOs;

public class CreateWalletRequestDto
{
    public string Name { get; set; } = null!;
    public string RegularType { get; set; } = RegularExchangeTypes.USD;
    public decimal InitialRegularBalance { get; set; }
    public string CryptoType { get; set; } = CryptoExchangeTypes.BTC;
    public decimal InitialCryptoBalance { get; set; }
}