using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Queries.GetRoleById
{
  public class GetRoleByIdQuery : IRequest<RoleDto>
  {
    public Guid RoleId { get; set; }
  }
}
