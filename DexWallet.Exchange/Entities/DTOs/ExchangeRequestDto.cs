namespace DexWallet.Exchange.Entities.DTOs;

public class ExchangeRequestDto
{
    public string WalletAddress { get; set; }
    public string FromType { get; set; }
    public decimal Amount { get; set; }
}