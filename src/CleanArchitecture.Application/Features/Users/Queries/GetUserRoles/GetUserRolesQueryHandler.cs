using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserRoles
{
  public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserRolesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
      var user = await _context.Users
          .Include(u => u.UserRoles)
              .ThenInclude(ur => ur.Role)
          .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

      if (user == null)
      {
        throw new UserNotFoundByIdError(request.UserId);
      }

      var roles = user.UserRoles.Select(ur => ur.Role).ToList();
      return _mapper.Map<List<RoleDto>>(roles);
    }
  }
}
