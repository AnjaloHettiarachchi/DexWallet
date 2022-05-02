using DexWallet.Identity.Entities.DTOs;
using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Contracts;

public interface IUserService
{
    Task<AuthenticateResponseDto> AuthenticateAsync(AuthenticateRequestDto request, string ipAddress);
    Task<AuthenticateResponseDto> RefreshTokenAsync(string token, string ipAddress);
    Task RevokeTokenAsync(string token, string ipAddress);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
}