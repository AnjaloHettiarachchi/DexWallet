using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DexWallet.Common;
using DexWallet.Core.Contracts;
using DexWallet.Core.Entities.Models;

namespace DexWallet.Core.Services;

public class WalletService : IWalletService
{
    private readonly IDynamoDBContext _dbContext;

    public WalletService(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Wallet>> GetAllWalletsByOwnerAsync(string owner)
    {
        var wallets = await _dbContext.ScanAsync<Wallet>(new List<ScanCondition>()).GetRemainingAsync();

        if (wallets.Count == 0)
            throw new AppException("No wallets found");

        return await _dbContext.ScanAsync<Wallet>(new List<ScanCondition> { new("Owner", ScanOperator.Equal, owner) })
            .GetRemainingAsync();
    }

    public Task<Wallet> GetWalletByAddressAsync(string owner, string walletAddress)
    {
        return _dbContext.LoadAsync<Wallet>(walletAddress, owner);
    }

    public async Task<Wallet> CreateWalletAsync(string owner, string name, string regularType, string cryptoType, decimal initialRegularBalance = 0M,
        decimal initialCryptoBalance = 0M)
    {
        var uniqueWalletAddress = Guid.NewGuid().ToString();

        var newWallet = new Wallet
        {
            Address = uniqueWalletAddress,
            Name = name,
            Owner = owner,
            CryptoBalance = initialCryptoBalance,
            CryptoType = cryptoType,
            RegularBalance = initialRegularBalance,
            RegularType = regularType
        };

        await _dbContext.SaveAsync(newWallet);

        return newWallet;
    }

    public async Task<Wallet> StoreCryptoFundsAsync(string owner, string walletAddress, decimal amount)
    {
        var wallet = await _dbContext.LoadAsync<Wallet>(walletAddress, owner);

        if (wallet is null)
            throw new AppException("Invalid wallet address");

        wallet.CryptoBalance += amount;

        await _dbContext.SaveAsync(wallet);

        return wallet;
    }

    public async Task<Wallet> StoreRegularFundsAsync(string owner, string walletAddress, decimal amount)
    {
        var wallet = await _dbContext.LoadAsync<Wallet>(walletAddress, owner);

        if (wallet is null)
            throw new AppException("Invalid wallet address");

        wallet.RegularBalance += amount;

        await _dbContext.SaveAsync(wallet);

        return wallet;
    }
}