using System.Text.Json.Serialization;

namespace DexWallet.Identity.Entities.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Username { get; init; } = null!;

    [JsonIgnore]
    public string PasswordHash { get; init; } = null!;

    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; } = null!;
}