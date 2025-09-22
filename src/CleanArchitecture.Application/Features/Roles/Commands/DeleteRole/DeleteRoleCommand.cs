using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Roles.Commands.DeleteRole
{
  public class DeleteRoleCommand : IRequest<Unit>
  {
    public Guid RoleId { get; set; }
  }
}
