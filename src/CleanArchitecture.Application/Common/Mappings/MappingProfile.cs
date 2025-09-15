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

            // Auth mappings
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
        }
    }
}
