using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Roles.Commands.UpdateRole
{
  public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateRoleCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
      var role = await _context.Roles
          .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

      if (role == null)
      {
        throw new RoleNotFoundByIdError(request.RoleId);
      }

      // Update only the allowed fields (Name and Description)
      // Note: PermissionIds are ignored in this update operation
      role.Name = request.Role.Name;
      role.Description = request.Role.Description;
      role.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);

      return _mapper.Map<RoleDto>(role);
    }
  }
}
