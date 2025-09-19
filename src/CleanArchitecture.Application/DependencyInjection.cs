using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
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
      // AutoMapper
      services.AddAutoMapper(Assembly.GetExecutingAssembly());

      // MediatR
      services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

      // FluentValidation
      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

      // Services
      services.AddScoped<IAuthService, AuthService>();
      services.AddScoped<IEmailService, EmailService>();
      services.AddScoped<IEmailTemplateService, EmailTemplateService>();
      services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();

      // Permission services
      services.AddScoped<IPermissionService, HierarchicalPermissionService>();

      services.AddScoped<ILocalizationService, LocalizationService>();
      services.AddScoped<IPaginationService, PaginationService>();
      services.AddScoped<IUserTimezoneService, UserTimezoneService>();

      // Add HttpContextAccessor for localization and timezone
      services.AddHttpContextAccessor();

      return services;
    }
  }
}
