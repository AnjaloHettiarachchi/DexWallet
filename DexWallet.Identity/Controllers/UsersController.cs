using DexWallet.Common.Attributes;
using DexWallet.Identity.Contracts;
using DexWallet.Identity.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DexWallet.Identity.Controllers;

[Authorize]
[ApiController]
[Route("/identity")]
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
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        return Ok(user);
    }

    [HttpGet("validate")]
    public IActionResult Validate()
    {
        return Ok(HttpContext.Items["User"]);
    }

    private string CurrentIpAddress()
    {
        // Get source IP Address from current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
    }
}