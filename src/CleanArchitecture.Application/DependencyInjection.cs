using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Services;
using CleanArchitecture.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
      // MediatR
      services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

      // FluentValidation
      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

      // Services
      services.AddScoped<IAuthService, AuthService>();
      services.AddScoped<IEmailService, EmailService>();

      // Permission services - HierarchicalPermissionService wraps the base PermissionService
      services.AddScoped<PermissionService>(); // Base service
      services.AddScoped<IPermissionService, HierarchicalPermissionService>(); // Hierarchical wrapper

      services.AddScoped<ILocalizationService, LocalizationService>();
      services.AddScoped<IPaginationService, PaginationService>();

      // Add HttpContextAccessor for localization
      services.AddHttpContextAccessor();

      return services;
    }
  }
}
