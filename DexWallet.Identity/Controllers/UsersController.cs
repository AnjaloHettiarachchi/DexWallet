using DexWallet.Common;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.DTOs;
using DexWallet.Identity.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DexWallet.Identity.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequestDto request)
    {
        var response = await _userService.AuthenticateAsync(request, CurrentIpAddress());
        SetHttpOnlyCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh-token"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { message = "Token is required" });

        var response = await _userService.RefreshTokenAsync(refreshToken, CurrentIpAddress());
        SetHttpOnlyCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request)
    {
        // Accept refresh token in request body or cookie
        var token = string.IsNullOrEmpty(request.Token) ? Request.Cookies["refresh-token"] : request.Token;

        if (string.IsNullOrEmpty(token))
            return BadRequest(new AppResponse(null, false, "Token is required"));

        await _userService.RevokeTokenAsync(token, CurrentIpAddress());
        return Ok(new AppResponse(null, true, "Token has been revoked"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpGet("{id:int}/refresh-tokens")]
    public async Task<IActionResult> GetRefreshTokens(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user.RefreshTokens);
    }

    private void SetHttpOnlyCookie(string token)
    {
        // Append Http-only Cookie with Refresh Token to the response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refresh-token", token, cookieOptions);
    }

    private string CurrentIpAddress()
    {
        // Get source IP Address from current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
    }
}