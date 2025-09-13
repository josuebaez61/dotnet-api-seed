using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Common.Services
{
  public class LocalizationService : ILocalizationService
  {
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LocalizationService(IStringLocalizer<SharedResource> localizer)
    {
      _localizer = localizer;
    }

    public string GetString(string key, params object[] args)
    {
      var localizedString = _localizer[key];
      return args.Length > 0 ? string.Format(localizedString, args) : localizedString;
    }

    public string GetSuccessMessage(string key, params object[] args)
    {
      return GetString($"Messages:Success:{key}", args);
    }

    public string GetErrorMessage(string key, params object[] args)
    {
      return GetString($"Messages:Errors:{key}", args);
    }

    public string GetValidationMessage(string key, params object[] args)
    {
      return GetString($"Messages:Validation:{key}", args);
    }
  }
}
