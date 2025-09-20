using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.API.Attributes
{
  /// <summary>
  /// Attribute that requires the user to have ANY of the specified permissions
  /// Usage: [RequireAnyPermission("manage.users", "admin")]
  /// </summary>
  public class RequireAnyPermissionAttribute : Attribute, IAsyncAuthorizationFilter
  {
    private readonly string[] _permissions;

    public RequireAnyPermissionAttribute(params string[] permissions)
    {
      _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
      if (_permissions.Length == 0)
        throw new ArgumentException("At least one permission must be specified", nameof(permissions));
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
      var user = context.HttpContext.User;

      if (!user.Identity?.IsAuthenticated ?? true)
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      var userPermissions = user.FindAll("permission").Select(c => c.Value).ToList();

      // Check if user has ANY of the required permissions
      var hasAnyPermission = _permissions.Any(requiredPermission =>
          userPermissions.Contains(requiredPermission));

      if (!hasAnyPermission)
      {
        // Return detailed error information
        var missingPermissions = string.Join(", ", _permissions);
        var userPermissionList = string.Join(", ", userPermissions);

        context.Result = new ForbidResult();

        // Log the authorization failure for debugging
        var logger = context.HttpContext.RequestServices.GetService<ILogger<RequireAnyPermissionAttribute>>();
        logger?.LogWarning("Authorization failed. Required any of: {RequiredPermissions}. User has: {UserPermissions}",
            missingPermissions, userPermissionList);
      }
    }
  }
}
