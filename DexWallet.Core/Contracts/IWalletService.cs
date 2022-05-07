using DexWallet.Core.Entities.Models;

namespace DexWallet.Core.Contracts;

public interface IWalletService
{
    Task<IEnumerable<Wallet>> GetAllWalletsByOwnerAsync(string owner);
    Task<Wallet> GetWalletByAddressAsync(string owner, string walletAddress);
    Task<Wallet> CreateWalletAsync(string owner, string name, string regularType, string cryptoType, decimal initialRegularBalance = decimal.Zero, decimal initialCryptoBalance = decimal.Zero);
    Task<Wallet> StoreCryptoFundsAsync(string owner, string walletAddress, decimal amount);
    Task<Wallet> StoreRegularFundsAsync(string owner, string walletAddress, decimal amount);
}