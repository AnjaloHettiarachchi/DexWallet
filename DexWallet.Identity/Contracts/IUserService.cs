using DexWallet.Identity.Entities.DTOs;
using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Contracts;

public interface IUserService
{
    Task<AuthenticateResponseDto> AuthenticateAsync(AuthenticateRequestDto request, string ipAddress);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByUsernameAsync(string username);
}