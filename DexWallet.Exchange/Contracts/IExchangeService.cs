using DexWallet.Core.Entities.Models;

namespace DexWallet.Exchange.Contracts;

public interface IExchangeService
{
    Task<Wallet> DoExchange(string authToken, string walletAddress, string fromType, decimal amount);
}