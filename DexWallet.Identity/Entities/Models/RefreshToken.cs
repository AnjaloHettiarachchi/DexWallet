// using Amazon.DynamoDBv2.DataModel;
//
// namespace DexWallet.Identity.Entities.Models;
//
// [DynamoDBTable("DexWallet.Identity.RefreshToken")]
// public class RefreshToken
// {
//     [DynamoDBHashKey]
//     public string Token { get; init; } = null!;
//
//     [DynamoDBProperty]
//     public DateTime Expires { get; init; }
//
//     [DynamoDBProperty]
//     public DateTime Created { get; init; }
//
//     [DynamoDBProperty]
//     public string CreatedByIp { get; init; } = null!;
//
//     [DynamoDBProperty]
//     public DateTime? Revoked { get; set; }
//
//     [DynamoDBProperty]
//     public string? RevokedByIp { get; set; }
//
//     [DynamoDBProperty]
//     public string? ReplacedByToken { get; set; }
//
//     [DynamoDBProperty]
//     public string? ReasonRevoked { get; set; }
//
//     private bool IsExpired => DateTime.UtcNow >= Expires;
//
//     [DynamoDBProperty]
//     private bool IsRevoked => Revoked.HasValue;
//
//     [DynamoDBProperty]
//     public bool IsActive => !IsRevoked && !IsExpired;
// }

