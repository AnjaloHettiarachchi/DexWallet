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

        var userId = jwtUtilities.ValidateToken(token);
        if (userId.HasValue) context.Items["User"] = await userService.GetByIdAsync(userId.Value);

        await _next(context);
    }
}