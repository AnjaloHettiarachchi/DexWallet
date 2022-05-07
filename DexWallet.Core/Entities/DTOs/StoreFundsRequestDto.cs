namespace DexWallet.Core.Entities.DTOs;

public class StoreFundsRequestDto
{
    public string WalletAddress { get; set; } = null!;
    public decimal Amount { get; set; }
}