using Amazon.DynamoDBv2.DataModel;
using DexWallet.Exchange.Contracts;
using DexWallet.Exchange.Entities.Models;

namespace DexWallet.Exchange.Services;

public class RateService : IRateService
{
    private readonly IDynamoDBContext _dbContext;

    public RateService(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<decimal> GetCryptoValueAsync(string regularType, string cryptoType, decimal amount)
    {
        var rate = await _dbContext.LoadAsync<Rate>(regularType, cryptoType);
        return amount * rate.Value;
    }

    public async Task<decimal> GetRegularValueAsync(string regularType, string cryptoType, decimal amount)
    {
        var rate = await _dbContext.LoadAsync<Rate>(regularType, cryptoType);
        return amount / rate.Value;
    }
}