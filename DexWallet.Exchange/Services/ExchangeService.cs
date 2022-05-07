using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common;
using DexWallet.Common.Clients;
using DexWallet.Core.Entities.Models;
using DexWallet.Exchange.Contracts;

namespace DexWallet.Exchange.Services;

public class ExchangeService : IExchangeService
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IRateService _rateService;
    private readonly WalletServiceClient _walletServiceClient;

    public ExchangeService(WalletServiceClient walletServiceClient, IRateService rateService, IDynamoDBContext dbContext)
    {
        _walletServiceClient = walletServiceClient;
        _rateService = rateService;
        _dbContext = dbContext;
    }

    public async Task<Wallet> DoExchange(string authToken, string walletAddress, string fromType, decimal amount)
    {
        var walletByAddressResponse = await _walletServiceClient.GetWalletByAddressAsync(authToken, walletAddress);

        if (walletByAddressResponse is null or { IsSuccess: false } or { Result: null })
            throw new AppException("Invalid wallet address");

        var wallet = new Wallet
        {
            Address = walletByAddressResponse.Result.Address,
            Name = walletByAddressResponse.Result.Name,
            Owner = walletByAddressResponse.Result.Owner,
            CryptoBalance = walletByAddressResponse.Result.CryptoBalance,
            CryptoType = walletByAddressResponse.Result.CryptoType,
            RegularBalance = walletByAddressResponse.Result.RegularBalance,
            RegularType = walletByAddressResponse.Result.RegularType
        };

        if (!fromType.Equals(wallet.RegularType) && !fromType.Equals(wallet.CryptoType))
            throw new AppException("Specified wallet does not support given currency type");

        if (fromType.Equals(wallet.RegularType))
        {
            if (wallet.RegularBalance < amount)
                throw new AppException("Not enough balance available");

            var cryptoValue = await _rateService.GetCryptoValueAsync(wallet.RegularType, wallet.CryptoType, amount);
            wallet.RegularBalance -= amount;
            wallet.CryptoBalance += cryptoValue;
        }
        else
        {
            if (wallet.CryptoBalance < amount)
                throw new AppException("Not enough balance available");

            var regularValue = await _rateService.GetRegularValueAsync(wallet.RegularType, wallet.CryptoType, amount);
            wallet.CryptoBalance -= amount;
            wallet.RegularBalance += Math.Round(regularValue, 2, MidpointRounding.AwayFromZero);
        }

        await _dbContext.SaveAsync(wallet);

        return wallet;
    }
}