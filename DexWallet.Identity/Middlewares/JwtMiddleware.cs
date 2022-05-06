using DexWallet.Identity.Contracts;

namespace DexWallet.Identity.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService, IJwtUtilities jwtUtilities)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;

        var username = jwtUtilities.ValidateToken(token);
        if (!string.IsNullOrEmpty(username)) context.Items["User"] = await userService.GetByUsernameAsync(username);

        await _next(context);
    }
}