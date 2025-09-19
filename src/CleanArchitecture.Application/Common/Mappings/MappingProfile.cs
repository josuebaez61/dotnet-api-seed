using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();

            // Country mappings
            CreateMap<Country, CountryDto>();

            // State mappings
            CreateMap<State, StateDto>();

            // City mappings
            CreateMap<City, CityDto>();

            // Role mappings
            CreateMap<Role, RoleDto>();

            // Permission mappings
            CreateMap<Permission, PermissionDto>();

            // RolePermission mappings
            CreateMap<RolePermission, RolePermissionDto>();


            CreateMap<User, AuthUserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role.Name)
                        .ToList()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.UserRoles
                        .Where(ur => ur.Role != null && ur.Role.RolePermissions != null)
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Where(rp => rp.Permission != null)
                        .Select(rp => rp.Permission.Name)
                        .Distinct()
                        .ToList()));


            // Auth mappings
            CreateMap<User, AuthDataDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
        }
    }
}
