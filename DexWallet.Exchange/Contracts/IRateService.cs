namespace DexWallet.Exchange.Contracts;

public interface IRateService
{
    Task<decimal> GetCryptoValueAsync(string regularType, string cryptoType, decimal amount);
    Task<decimal> GetRegularValueAsync(string regularType, string cryptoType, decimal amount);
}