using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Entities.DTOs;

public class AuthenticateResponseDto
{
    public AuthenticateResponseDto(User user, string jwtToken)
    {
        // Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Username;
        Token = jwtToken;
    }

    // public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Username { get; }
    public string Token { get; }
}