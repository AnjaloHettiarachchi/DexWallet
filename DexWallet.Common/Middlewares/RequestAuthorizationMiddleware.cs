using DexWallet.Common.Clients;
using Microsoft.AspNetCore.Http;

namespace DexWallet.Common.Middlewares;

public class RequestAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RequestAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IdentityServiceClient identityServiceClient)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last() ?? string.Empty;

        // Call Identity Service and validate token
        var response = await identityServiceClient.ValidateTokenAsync(token);
        if (response.IsSuccess) context.Items["User"] = response.Result;

        await _next(context);
    }
}