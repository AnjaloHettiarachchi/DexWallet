using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common.Entities;

namespace DexWallet.Core.Entities.Models;

[DynamoDBTable("DexWallet.Core.Wallet", true)]
public class Wallet
{
    [DynamoDBHashKey]
    public string Address { get; set; } = null!;

    [DynamoDBRangeKey]
    public string Owner { get; set; } = null!;

    [DynamoDBProperty]
    public string Name { get; set; } = null!;

    [DynamoDBProperty]
    public string CryptoType { get; set; } = CryptoExchangeTypes.BTC;

    [DynamoDBProperty]
    public decimal CryptoBalance { get; set; }

    [DynamoDBProperty]
    public string RegularType { get; set; } = RegularExchangeTypes.USD;

    [DynamoDBProperty]
    public decimal RegularBalance { get; set; }
}