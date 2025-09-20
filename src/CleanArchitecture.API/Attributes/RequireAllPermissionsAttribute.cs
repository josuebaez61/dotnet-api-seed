using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.API.Attributes
{
  /// <summary>
  /// Attribute that requires the user to have ALL of the specified permissions
  /// Usage: [RequireAllPermissions("manage.users", "manage.roles")]
  /// </summary>
  public class RequireAllPermissionsAttribute : Attribute, IAsyncAuthorizationFilter
  {
    private readonly string[] _permissions;

    public RequireAllPermissionsAttribute(params string[] permissions)
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

      // Check if user has ALL of the required permissions
      var hasAllPermissions = _permissions.All(requiredPermission =>
          userPermissions.Contains(requiredPermission));

      if (!hasAllPermissions)
      {
        // Return detailed error information
        var missingPermissions = _permissions.Where(p => !userPermissions.Contains(p)).ToList();
        var userPermissionList = string.Join(", ", userPermissions);

        context.Result = new ForbidResult();

        // Log the authorization failure for debugging
        var logger = context.HttpContext.RequestServices.GetService<ILogger<RequireAllPermissionsAttribute>>();
        logger?.LogWarning("Authorization failed. Required all of: {RequiredPermissions}. User missing: {MissingPermissions}. User has: {UserPermissions}",
            string.Join(", ", _permissions), string.Join(", ", missingPermissions), userPermissionList);
      }
    }
  }
}
