using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DexWallet.Identity.Helpers;

public class JwtUtilities : IJwtUtilities
{
    private readonly AppSettings _appSettings;
    private readonly IDynamoDBContext _dataContext;

    public JwtUtilities(IDynamoDBContext dataContext, IOptions<AppSettings> appSettings)
    {
        _dataContext = dataContext;
        _appSettings = appSettings.Value;
    }

    /// <summary>
    ///     Generate JWT Token that is valid for 15 minutes.
    /// </summary>
    /// <param name="user">User instance.</param>
    /// <returns>Generated JWT token as a string.</returns>
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.SigningKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", user.Username) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.SigningKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(x => x.Type == "username").Value;

            return username;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GetUniqueTokenAsync(bool hasPreviousTokens)
    {
        while (true)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            if (!hasPreviousTokens) return token;

            var usersWithCurrentToken = await _dataContext
                .ScanAsync<User>(new List<ScanCondition> { new("refreshTokens", ScanOperator.Contains, token) })
                .GetRemainingAsync();

            if (usersWithCurrentToken.Any()) continue;
            return token;
        }
    }
}