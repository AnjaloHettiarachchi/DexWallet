using Amazon.DynamoDBv2.DataModel;
using DexWallet.Common;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.DTOs;
using DexWallet.Identity.Entities.Models;

namespace DexWallet.Identity.Services;

public class UserService : IUserService
{
    private readonly IDynamoDBContext _dataContext;
    private readonly IJwtUtilities _jwtUtilities;

    public UserService(IDynamoDBContext dataContext, IJwtUtilities jwtUtilities)
    {
        _dataContext = dataContext;
        _jwtUtilities = jwtUtilities;
    }

    public async Task<AuthenticateResponseDto> AuthenticateAsync(AuthenticateRequestDto request, string ipAddress)
    {
        var user = await _dataContext.LoadAsync<User>(request.Username);

        // Validate
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AppException("Invalid username or password");

        // Authentication successful. Generate a JWT and a Refresh Token
        var jwtToken = _jwtUtilities.GenerateToken(user);

        // Save changes in DB
        await _dataContext.SaveAsync(user);

        return new AuthenticateResponseDto(user, jwtToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dataContext.ScanAsync<User>(new List<ScanCondition>()).GetRemainingAsync();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var user = await _dataContext.LoadAsync<User>(username);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}