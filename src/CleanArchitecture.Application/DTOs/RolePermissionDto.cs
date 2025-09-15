using System;

namespace CleanArchitecture.Application.DTOs
{
    public class RolePermissionDto
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
