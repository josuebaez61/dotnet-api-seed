using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoleUserCount
{
  public class GetRoleUserCountQueryHandler : IRequestHandler<GetRoleUserCountQuery, RoleUserCountDto>
  {
    private readonly IApplicationDbContext _context;

    public GetRoleUserCountQueryHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<RoleUserCountDto> Handle(GetRoleUserCountQuery request, CancellationToken cancellationToken)
    {
      // Get user count per role from UserRoles table
      var roleUserCounts = await _context.UserRoles
          .GroupBy(ur => ur.RoleId)
          .Select(g => new { RoleId = g.Key, UserCount = g.Count() })
          .ToListAsync(cancellationToken);

      // Convert to dictionary format
      var result = new RoleUserCountDto();

      foreach (var item in roleUserCounts)
      {
        result[item.RoleId] = item.UserCount;
      }

      return result;
    }
  }
}
