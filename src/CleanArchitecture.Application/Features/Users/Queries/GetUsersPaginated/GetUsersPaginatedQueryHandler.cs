using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Common.Constants;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUsersPaginated
{
  public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginationResponse<UserDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IPaginationService _paginationService;

    private readonly IMapper _mapper;

    public GetUsersPaginatedQueryHandler(
        IApplicationDbContext context,
        IPaginationService paginationService,
        IMapper mapper
      )
    {
      _context = context;
      _paginationService = paginationService;
      _mapper = mapper;
    }

    public async Task<PaginationResponse<UserDto>> Handle(
        GetUsersPaginatedQuery request,
        CancellationToken cancellationToken)
    {
      var query = _context.Users.AsQueryable();

      // Aplicar filtros específicos
      query = ApplyFilters(query, request.Request);

      // Convertir a DTO antes de paginar
      var dtoQuery = query.Select(user => new UserDto
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        UserName = user.UserName!,
        DateOfBirth = user.DateOfBirth,
        ProfilePicture = user.ProfilePicture,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        IsActive = user.IsActive,
        EmailConfirmed = user.EmailConfirmed,
        Roles = user.UserRoles.Select(ur => new RoleDto
        {
          Id = ur.Role.Id,
          Name = ur.Role.Name!,
          Description = ur.Role.Description,
        }).ToList()
      });

      // Aplicar paginación
      var result = await _paginationService.GetPaginatedAsync(dtoQuery, request.Request, cancellationToken);

      return result;
    }

    private IQueryable<User> ApplyFilters(IQueryable<User> query, GetUsersPaginatedRequestDto request)
    {
      // Filtro por nombre de usuario
      if (!string.IsNullOrEmpty(request.UserName))
      {
        query = query.Where(u => u.UserName!.ToLower().Contains(request.UserName.ToLower()));
      }

      // Filtro por email
      if (!string.IsNullOrEmpty(request.Email))
      {
        query = query.Where(u => u.Email!.ToLower().Contains(request.Email.ToLower()));
      }

      // Filtro por nombre
      if (!string.IsNullOrEmpty(request.FirstName))
      {
        query = query.Where(u => u.FirstName.ToLower().Contains(request.FirstName.ToLower()));
      }

      // Filtro por apellido
      if (!string.IsNullOrEmpty(request.LastName))
      {
        query = query.Where(u => u.LastName.ToLower().Contains(request.LastName.ToLower()));
      }

      // Filtro por estado activo
      if (request.IsActive != null)
      {
        if (request.IsActive == "true")
        {
          query = query.Where(u => u.IsActive == true);
        }
        else if (request.IsActive == "false")
        {
          query = query.Where(u => u.IsActive == false);
        }
      }

      // Filtro por confirmación de email
      if (request.EmailConfirmed.HasValue)
      {
        query = query.Where(u => u.EmailConfirmed == request.EmailConfirmed.Value);
      }

      // Filtro por fecha de creación (desde)
      if (request.CreatedFrom.HasValue)
      {
        query = query.Where(u => u.CreatedAt >= request.CreatedFrom.Value);
      }

      // Filtro por fecha de creación (hasta)
      if (request.CreatedTo.HasValue)
      {
        query = query.Where(u => u.CreatedAt <= request.CreatedTo.Value);
      }

      // Filtro por RoleId
      if (request.RoleId.HasValue)
      {
        var filterMode = request.RoleIdFilterMatchMode ?? FilterMatchMode.Contains;

        switch (filterMode)
        {
          case FilterMatchMode.Contains:
            // Usuarios que tienen el rol especificado
            query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            break;

          case FilterMatchMode.NotContains:
            // Usuarios que NO tienen el rol especificado
            query = query.Where(u => !u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            break;

          default:
            // Por defecto, usar Contains
            query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            break;
        }
      }

      return query;
    }
  }
}
