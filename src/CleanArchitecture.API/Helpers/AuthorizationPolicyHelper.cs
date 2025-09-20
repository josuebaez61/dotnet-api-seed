using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.API.Helpers
{
    /// <summary>
    /// Helper class for creating dynamic authorization policies
    /// </summary>
    public static class AuthorizationPolicyHelper
    {
        /// <summary>
        /// Creates a policy that requires ANY of the specified permissions
        /// </summary>
        /// <param name="permissions">The permissions to check</param>
        /// <returns>Policy name</returns>
        public static string CreateAnyPermissionPolicy(params string[] permissions)
        {
            var policyName = $"any_{string.Join("_or_", permissions)}";
            
            // This would be used with a custom authorization service
            // For now, we'll return the policy name
            return policyName;
        }

        /// <summary>
        /// Creates a policy that requires ALL of the specified permissions
        /// </summary>
        /// <param name="permissions">The permissions to check</param>
        /// <returns>Policy name</returns>
        public static string CreateAllPermissionPolicy(params string[] permissions)
        {
            var policyName = $"all_{string.Join("_and_", permissions)}";
            
            // This would be used with a custom authorization service
            // For now, we'll return the policy name
            return policyName;
        }

        /// <summary>
        /// Adds dynamic authorization policies to the service collection
        /// </summary>
        public static IServiceCollection AddDynamicAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, DynamicPermissionHandler>();
            
            return services;
        }
    }

    /// <summary>
    /// Custom authorization handler for dynamic permission policies
    /// </summary>
    public class DynamicPermissionHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                if (requirement is DynamicPermissionRequirement dynamicReq)
                {
                    var userPermissions = context.User.FindAll("permission").Select(c => c.Value).ToList();

                    bool hasPermission = dynamicReq.RequireAll 
                        ? dynamicReq.Permissions.All(p => userPermissions.Contains(p))
                        : dynamicReq.Permissions.Any(p => userPermissions.Contains(p));

                    if (hasPermission)
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Requirement for dynamic permission policies
    /// </summary>
    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
        public string[] Permissions { get; }
        public bool RequireAll { get; }

        public DynamicPermissionRequirement(string[] permissions, bool requireAll = false)
        {
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            RequireAll = requireAll;
        }
    }
}
