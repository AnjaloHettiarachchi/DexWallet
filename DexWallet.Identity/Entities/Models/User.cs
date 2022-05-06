using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace DexWallet.Identity.Entities.Models;

[DynamoDBTable("DexWallet.Identity.User", true)]
public class User
{
    [DynamoDBHashKey]
    public string Username { get; init; } = null!;

    [DynamoDBProperty]
    public string FirstName { get; init; } = null!;

    [DynamoDBProperty]
    public string LastName { get; init; } = null!;

    [JsonIgnore]
    [DynamoDBProperty]
    public string PasswordHash { get; init; } = null!;
}