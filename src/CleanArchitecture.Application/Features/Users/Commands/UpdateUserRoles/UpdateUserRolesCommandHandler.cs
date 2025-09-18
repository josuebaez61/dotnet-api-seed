using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUserRoles
{
  public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, List<RoleDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateUserRolesCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
      // Verificar que el usuario existe
      var user = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      // Verificar que todos los roles existen
      var existingRoles = await _context.Roles
          .Where(r => request.RoleIds.Contains(r.Id))
          .ToListAsync(cancellationToken);

      if (existingRoles.Count != request.RoleIds.Count)
      {
        var foundRoleIds = existingRoles.Select(r => r.Id).ToList();
        var missingRoleIds = request.RoleIds.Except(foundRoleIds).ToList();
        throw new ArgumentException($"Roles not found: {string.Join(", ", missingRoleIds)}");
      }

      // Remover roles existentes del usuario
      var currentUserRoles = await _context.UserRoles
          .Where(ur => ur.UserId == request.UserId)
          .ToListAsync(cancellationToken);

      _context.UserRoles.RemoveRange(currentUserRoles);

      // Agregar nuevos roles al usuario
      var newUserRoles = request.RoleIds.Select(roleId => new UserRole
      {
        UserId = request.UserId,
        RoleId = roleId
      }).ToList();

      await _context.UserRoles.AddRangeAsync(newUserRoles, cancellationToken);

      await _context.SaveChangesAsync(cancellationToken);

      // Retornar los roles actualizados
      var updatedRoles = await _context.Roles
          .Where(r => request.RoleIds.Contains(r.Id))
          .ToListAsync(cancellationToken);

      return _mapper.Map<List<RoleDto>>(updatedRoles);
    }
  }
}
