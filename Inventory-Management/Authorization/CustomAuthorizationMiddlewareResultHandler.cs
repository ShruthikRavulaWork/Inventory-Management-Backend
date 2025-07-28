using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Text.Json;

namespace InventoryAPI.Authorization
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
            {
                await next(context);
                return;
            }


            context.Response.ContentType = "application/json";

            // If the user is not authenticated (401 Unauthorized)
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = JsonSerializer.Serialize(new
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "You need to log in to access this resource."
                });
                await context.Response.WriteAsync(result);
                return;
            }

            // If the user is authenticated but not authorized (403 Forbidden)
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                var endpoint = context.GetEndpoint();
                var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
                var requiredRoles = authorizeAttribute?.Roles;

                string message;
                if (!string.IsNullOrEmpty(requiredRoles))
                {
                    message = $"Access Denied. This resource requires the following role(s): '{requiredRoles}'.";
                }
                else
                {
                    message = "Access Denied. You do not have the required permissions for this resource.";
                }

                var result = JsonSerializer.Serialize(new
                {
                    statusCode = StatusCodes.Status403Forbidden,
                    message = message
                });
                await context.Response.WriteAsync(result);
                return;
            }
        }
    }
}