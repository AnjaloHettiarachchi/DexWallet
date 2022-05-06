using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Contracts;

public interface IJwtUtilities
{
    string GenerateToken(User user);
    string? ValidateToken(string token);
}