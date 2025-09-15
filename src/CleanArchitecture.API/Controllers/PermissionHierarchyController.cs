using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Configurations;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class PermissionHierarchyController : ControllerBase
  {
    /// <summary>
    /// Gets the hierarchical permissions for a specific permission
    /// </summary>
    /// <param name="permissionName">The permission to get hierarchical permissions for</param>
    /// <returns>List of permissions that are included hierarchically</returns>
    [HttpGet("id/hierarchical/{permissionName}")]
    [Authorize(Policy = PermissionConstants.Permissions.Read)]
    public ActionResult<ApiResponse<List<string>>> GetHierarchicalPermissions(string permissionName)
    {
      try
      {
        var hierarchicalPermissions = HierarchicalPermissionConfiguration.GetHierarchicalPermissions(permissionName);

        return Ok(ApiResponse<List<string>>.SuccessResponse(hierarchicalPermissions,
            $"Hierarchical permissions for {permissionName}"));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<string>>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Gets all parent permissions that include a specific permission hierarchically
    /// </summary>
    /// <param name="permissionName">The permission to find parents for</param>
    /// <returns>List of permissions that include this permission hierarchically</returns>
    [HttpGet("id/parents/{permissionName}")]
    [Authorize(Policy = PermissionConstants.Permissions.Read)]
    public ActionResult<ApiResponse<List<string>>> GetParentPermissions(string permissionName)
    {
      try
      {
        var parentPermissions = HierarchicalPermissionConfiguration.GetParentPermissions(permissionName);

        return Ok(ApiResponse<List<string>>.SuccessResponse(parentPermissions,
            $"Parent permissions for {permissionName}"));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<string>>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Validates the permission hierarchy configuration
    /// </summary>
    /// <returns>List of any invalid permissions found in the hierarchy</returns>
    [HttpGet("validate")]
    [Authorize(Policy = PermissionConstants.System.Admin)]
    public ActionResult<ApiResponse<List<string>>> ValidateHierarchy()
    {
      try
      {
        var validationErrors = HierarchicalPermissionConfiguration.ValidateHierarchy();

        if (validationErrors.Count == 0)
        {
          return Ok(ApiResponse<List<string>>.SuccessResponse(validationErrors,
              "Permission hierarchy validation passed"));
        }
        else
        {
          return BadRequest(ApiResponse<List<string>>.ErrorResponse("Permission hierarchy validation failed", validationErrors));
        }
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<List<string>>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Gets the complete permission hierarchy configuration
    /// </summary>
    /// <returns>Complete hierarchy configuration</returns>
    [HttpGet("configuration")]
    [Authorize(Policy = PermissionConstants.System.Admin)]
    public ActionResult<ApiResponse<object>> GetHierarchyConfiguration()
    {
      try
      {
        var configuration = new
        {
          TotalHierarchies = HierarchicalPermissionConfiguration.PermissionHierarchy.Count,
          Hierarchies = HierarchicalPermissionConfiguration.PermissionHierarchy,
          ValidationErrors = HierarchicalPermissionConfiguration.ValidateHierarchy()
        };

        return Ok(ApiResponse<object>.SuccessResponse(configuration,
            "Permission hierarchy configuration"));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
      }
    }

    /// <summary>
    /// Demonstrates how hierarchical permissions work with examples
    /// </summary>
    /// <returns>Examples of hierarchical permission behavior</returns>
    [HttpGet("examples")]
    [Authorize(Policy = PermissionConstants.Permissions.Read)]
    public ActionResult<ApiResponse<object>> GetExamples()
    {
      try
      {
        var examples = new
        {
          Description = "Examples of how hierarchical permissions work",
          Examples = new List<object>
                    {
                        new
                        {
                            Permission = PermissionConstants.Users.Write,
                            Includes = HierarchicalPermissionConfiguration.GetHierarchicalPermissions(PermissionConstants.Users.Write),
                            Explanation = "Users.Write automatically includes Users.Read"
                        },
                        new
                        {
                            Permission = PermissionConstants.Users.ManageRoles,
                            Includes = HierarchicalPermissionConfiguration.GetHierarchicalPermissions(PermissionConstants.Users.ManageRoles),
                            Explanation = "Users.ManageRoles includes Users.Read, Users.Write, Users.Update, and Roles.Read"
                        },
                        new
                        {
                            Permission = PermissionConstants.System.Admin,
                            Includes = HierarchicalPermissionConfiguration.GetHierarchicalPermissions(PermissionConstants.System.Admin),
                            Explanation = "System.Admin includes ALL system permissions"
                        },
                        new
                        {
                            Permission = PermissionConstants.Users.Read,
                            Parents = HierarchicalPermissionConfiguration.GetParentPermissions(PermissionConstants.Users.Read),
                            Explanation = "Users.Read is included by Users.Write, Users.Update, Users.Delete, Users.ManageRoles, Users.ViewSensitive, and System.Admin"
                        }
                    }
        };

        return Ok(ApiResponse<object>.SuccessResponse(examples,
            "Hierarchical permission examples"));
      }
      catch (Exception ex)
      {
        return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
      }
    }
  }
}
