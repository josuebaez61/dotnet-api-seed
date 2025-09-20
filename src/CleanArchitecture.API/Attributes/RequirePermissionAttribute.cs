using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.API.Attributes
{
  /// <summary>
  /// Flexible attribute that can require ANY or ALL permissions
  /// Usage: [RequirePermission(RequireMode.Any, "manage.users", "admin")]
  ///        [RequirePermission(RequireMode.All, "manage.users", "manage.roles")]
  /// </summary>
  public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
  {
    public enum RequireMode
    {
      Any,  // User needs ANY of the specified permissions
      All   // User needs ALL of the specified permissions
    }

    private readonly RequireMode _mode;
    private readonly string[] _permissions;

    public RequirePermissionAttribute(RequireMode mode, params string[] permissions)
    {
      _mode = mode;
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

      bool hasPermission = _mode switch
      {
        RequireMode.Any => _permissions.Any(requiredPermission =>
            userPermissions.Contains(requiredPermission)),
        RequireMode.All => _permissions.All(requiredPermission =>
            userPermissions.Contains(requiredPermission)),
        _ => false
      };

      if (!hasPermission)
      {
        context.Result = new ForbidResult();

        // Log the authorization failure for debugging
        var logger = context.HttpContext.RequestServices.GetService<ILogger<RequirePermissionAttribute>>();
        var requiredPermissions = string.Join(", ", _permissions);
        var userPermissionList = string.Join(", ", userPermissions);

        if (_mode == RequireMode.All)
        {
          var missingPermissions = _permissions.Where(p => !userPermissions.Contains(p)).ToList();
          logger?.LogWarning("Authorization failed. Required ALL of: {RequiredPermissions}. User missing: {MissingPermissions}. User has: {UserPermissions}",
              requiredPermissions, string.Join(", ", missingPermissions), userPermissionList);
        }
        else
        {
          logger?.LogWarning("Authorization failed. Required ANY of: {RequiredPermissions}. User has: {UserPermissions}",
              requiredPermissions, userPermissionList);
        }
      }
    }
  }
}
