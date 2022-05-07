using Amazon.DynamoDBv2.DataModel;

namespace DexWallet.Exchange.Entities.Models;

[DynamoDBTable("DexWallet.Exchange.Rate", true)]
public class Rate
{
    [DynamoDBHashKey]
    public string RegularType { get; set; } = null!;

    [DynamoDBRangeKey]
    public string CryptoType { get; set; } = null!;

    [DynamoDBProperty]
    public decimal Value { get; set; }
}