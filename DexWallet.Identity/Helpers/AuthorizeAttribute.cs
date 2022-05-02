using DexWallet.Identity.Entities.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DexWallet.Identity.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Skip if action is decorated with [AllowAnonymous] attribute
        var isAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (isAllowAnonymous) return;

        // Authorization
        if (context.HttpContext.Items["User"] is not User)
            throw new UnauthorizedAccessException("Unauthorized");
    }
}