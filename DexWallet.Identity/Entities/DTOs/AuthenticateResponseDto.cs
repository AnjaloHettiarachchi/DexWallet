using System.Text.Json.Serialization;
using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Entities.DTOs;

public class AuthenticateResponseDto
{
    public AuthenticateResponseDto(User user, string jwtToken, string refreshToken)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Username;
        Token = jwtToken;
        RefreshToken = refreshToken;
    }

    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Username { get; }
    public string Token { get; }

    [JsonIgnore]
    public string RefreshToken { get; }
}