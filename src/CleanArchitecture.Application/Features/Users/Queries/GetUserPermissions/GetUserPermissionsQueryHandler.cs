using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserPermissions
{
  public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<PermissionDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetUserPermissionsQueryHandler(IApplicationDbContext context, IPermissionService permissionService, IMapper mapper)
    {
      _context = context;
      _permissionService = permissionService;
      _mapper = mapper;
    }

    public async Task<List<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
      // Check if user exists first
      var userExists = await _context.Users
          .AnyAsync(u => u.Id == request.UserId, cancellationToken);

      if (!userExists)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Get user permissions through roles
      var permissions = await _permissionService.GetUserPermissionsAsync(request.UserId);

      return _mapper.Map<List<PermissionDto>>(permissions);
    }
  }
}
