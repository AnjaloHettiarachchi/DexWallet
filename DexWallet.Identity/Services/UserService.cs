using DexWallet.Common;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.DTOs;
using DexWallet.Identity.Entities.Models;
using DexWallet.Identity.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DexWallet.Identity.Services;

public class UserService : IUserService
{
    private readonly AppSettings _appSettings;
    private readonly DataContext _dataContext;
    private readonly IJwtUtilities _jwtUtilities;

    public UserService(DataContext dataContext, IJwtUtilities jwtUtilities, IOptions<AppSettings> appSettings)
    {
        _dataContext = dataContext;
        _jwtUtilities = jwtUtilities;
        _appSettings = appSettings.Value;
    }

    public async Task<AuthenticateResponseDto> AuthenticateAsync(AuthenticateRequestDto request, string ipAddress)
    {
        var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Username == request.Username);

        // Validate
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AppException("Invalid username or password");

        // Authentication successful. Generate a JWT and a Refresh Token
        var jwtToken = _jwtUtilities.GenerateToken(user);
        var refreshToken = _jwtUtilities.GenerateRefreshToken(ipAddress);
        user.RefreshTokens.Add(refreshToken);

        // Remove old refresh tokens from user
        RemoveOldRefreshTokens(user);

        // Save changes in DB
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();

        return new AuthenticateResponseDto(user, jwtToken, refreshToken.Token);
    }

    public async Task<AuthenticateResponseDto> RefreshTokenAsync(string token, string ipAddress)
    {
        var user = await GetUserByRefreshTokenAsync(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            // Revoke all descendant token in case this token has been compromised
            RevokeAllDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();
        }

        if (!refreshToken.IsActive) throw new AppException("Invalid token");

        // Replace old refresh token with a new one (Token Rotation)
        var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);

        // Remove old refresh tokens from user
        RemoveOldRefreshTokens(user);

        // Save changes to DB
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();

        // Generate new JWT
        var jwtToken = _jwtUtilities.GenerateToken(user);

        return new AuthenticateResponseDto(user, jwtToken, newRefreshToken.Token);
    }

    public async Task RevokeTokenAsync(string token, string? ipAddress)
    {
        var user = await GetUserByRefreshTokenAsync(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive) throw new AppException("Invalid token");

        // Revoke token and save to DB
        RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _dataContext.Update(user);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dataContext.Users.ToListAsync();
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var user = await _dataContext.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private void RemoveOldRefreshTokens(User user)
    {
        user.RefreshTokens.RemoveAll(x =>
            !x.IsActive && x.Created.AddDays(_appSettings.RefreshTokenTtl) <= DateTime.UtcNow);
    }

    private async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _dataContext.Users.SingleOrDefaultAsync(u =>
            u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null) throw new AppException("Invalid token");

        return user;
    }

    private static void RevokeAllDescendantRefreshTokens(RefreshToken refreshToken, User user, string? ipAddress, string? reason)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;

        // Recursively travel all Refresh Token chain and ensure all descendants are revoked
        var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);

        if (childToken == null) return;

        if (childToken.IsActive)
            RevokeRefreshToken(childToken, ipAddress, reason);
        else
            RevokeAllDescendantRefreshTokens(childToken, user, ipAddress, reason);
    }

    private static void RevokeRefreshToken(RefreshToken refreshToken, string? ipAddress, string? reason = null, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        refreshToken.RevokedByIp = ipAddress;
    }

    private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtilities.GenerateRefreshToken(ipAddress);
        RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }
}