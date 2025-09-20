using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Common.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetPermissionsByResource
{
  public class GetPermissionsByResourceQueryHandler : IRequestHandler<GetPermissionsByResourceQuery, List<PermissionsByResourceDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPermissionsByResourceQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<PermissionsByResourceDto>> Handle(GetPermissionsByResourceQuery request, CancellationToken cancellationToken)
    {
      // Get all permissions from database
      var permissions = await _context.Permissions
          .ToListAsync(cancellationToken);

      // Get ordered resources
      var orderedResources = PermissionResource.GetOrderedResources();

      // Group permissions by resource
      var groupedPermissions = permissions
          .GroupBy(p => p.Resource.ToLower())
          .ToList();

      var result = new List<PermissionsByResourceDto>();

      foreach (var orderedResource in orderedResources)
      {
        var resourceGroup = groupedPermissions
            .FirstOrDefault(g => g.Key.Equals(orderedResource.ToLower(), StringComparison.OrdinalIgnoreCase));

        var permissionsForResource = resourceGroup?.ToList() ?? new List<Domain.Entities.Permission>();

        var dto = new PermissionsByResourceDto
        {
          Resource = orderedResource,
          Order = orderedResources.IndexOf(orderedResource),
          Permissions = _mapper.Map<List<PermissionDto>>(permissionsForResource)
        };

        result.Add(dto);
      }

      // Add any resources that exist in the database but not in the ordered list
      var missingResources = groupedPermissions
          .Where(g => !orderedResources.Any(or => or.Equals(g.Key, StringComparison.OrdinalIgnoreCase)))
          .ToList();

      foreach (var missingResource in missingResources)
      {
        var dto = new PermissionsByResourceDto
        {
          Resource = missingResource.Key,
          Order = orderedResources.Count, // Add at the end
          Permissions = _mapper.Map<List<PermissionDto>>(missingResource.ToList())
        };

        result.Add(dto);
      }

      return result.OrderBy(r => r.Order).ToList();
    }
  }
}
