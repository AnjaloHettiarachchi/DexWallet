using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace DexWallet.Identity.Entities.Models;

[Owned]
public class RefreshToken
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    public string Token { get; init; } = null!;
    public DateTime Expires { get; init; }
    public DateTime Created { get; init; }
    public string CreatedByIp { get; init; } = null!;
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    private bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;
}