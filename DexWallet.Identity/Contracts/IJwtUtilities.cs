using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Contracts;

public interface IJwtUtilities
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
    RefreshToken GenerateRefreshToken(string ipAddress);
}