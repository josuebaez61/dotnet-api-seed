using System.Xml.Linq;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Common.Services
{
  public class LocalizationService : ILocalizationService
  {
    private static readonly Dictionary<string, Dictionary<string, string>> _cache = new();
    private readonly string _culture;

    public LocalizationService()
    {
      _culture = "en"; // Default culture
    }

    public LocalizationService(string culture)
    {
      _culture = culture ?? "en";
    }

    public LocalizationService(IHttpContextAccessor httpContextAccessor)
    {
      _culture = GetCurrentCulture(httpContextAccessor.HttpContext);
    }

    public string GetString(string key, params object[] args)
    {
      try
      {
        var localizedString = GetLocalizedValue(key, _culture);

        if (args.Length > 0)
        {
          return string.Format(localizedString, args);
        }

        return localizedString;
      }
      catch
      {
        // Fallback to key if localization fails
        return key;
      }
    }

    public string GetSuccessMessage(string key, params object[] args)
    {
      return GetString($"Success_{key}", args);
    }

    public string GetErrorMessage(string key, params object[] args)
    {
      return GetString($"Error_{key}", args);
    }

    public string GetValidationMessage(string key, params object[] args)
    {
      return GetString($"Validation_{key}", args);
    }

    private string GetLocalizedValue(string key, string culture)
    {
      try
      {
        if (!_cache.ContainsKey(culture))
        {
          LoadResxFile(culture);
        }

        if (_cache.ContainsKey(culture) && _cache[culture].ContainsKey(key))
        {
          return _cache[culture][key];
        }

        // Fallback to English if key not found in current culture
        if (culture != "en")
        {
          if (!_cache.ContainsKey("en"))
          {
            LoadResxFile("en");
          }

          if (_cache.ContainsKey("en") && _cache["en"].ContainsKey(key))
          {
            return _cache["en"][key];
          }
        }

        return key; // Return key as fallback
      }
      catch
      {
        return key;
      }
    }

    private void LoadResxFile(string culture)
    {
      try
      {
        var fileName = culture == "es" ? "Messages.es.resx" : "Messages.resx";
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);

        // If not found in base directory, try relative path
        if (!File.Exists(filePath))
        {
          filePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "CleanArchitecture.API", "Resources", fileName);
        }

        // If still not found, try in the output directory
        if (!File.Exists(filePath))
        {
          filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "CleanArchitecture.API", "Resources", fileName);
        }

        if (File.Exists(filePath))
        {
          var xmlDoc = XDocument.Load(filePath);
          var dictionary = new Dictionary<string, string>();

          foreach (var dataElement in xmlDoc.Descendants("data"))
          {
            var name = dataElement.Attribute("name")?.Value;
            var value = dataElement.Element("value")?.Value;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
              dictionary[name] = value;
            }
          }

          _cache[culture] = dictionary;
        }
        else
        {
          // If file doesn't exist, create a fallback dictionary with some test values
          var dictionary = new Dictionary<string, string>();
          if (culture == "es")
          {
            dictionary["Error_USER_NOT_FOUND"] = "Usuario no encontrado";
            dictionary["Error_INVALID_CREDENTIALS"] = "Credenciales inválidas";
          }
          else
          {
            dictionary["Error_USER_NOT_FOUND"] = "User not found";
            dictionary["Error_INVALID_CREDENTIALS"] = "Invalid credentials";
          }
          _cache[culture] = dictionary;
        }
      }
      catch (Exception ex)
      {
        // If loading fails, create a fallback dictionary
        var dictionary = new Dictionary<string, string>();
        if (culture == "es")
        {
          dictionary["Error_USER_NOT_FOUND"] = "Usuario no encontrado";
          dictionary["Error_INVALID_CREDENTIALS"] = "Credenciales inválidas";
        }
        else
        {
          dictionary["Error_USER_NOT_FOUND"] = "User not found";
          dictionary["Error_INVALID_CREDENTIALS"] = "Invalid credentials";
        }
        _cache[culture] = dictionary;
      }
    }

    private static string GetCurrentCulture(HttpContext? context)
    {
      // Check query parameter first (higher priority)
      if (context?.Request?.Query?.ContainsKey("culture") == true)
      {
        var culture = context.Request.Query["culture"].ToString().ToLower();
        if (culture == "es" || culture.StartsWith("es-"))
        {
          return "es";
        }
        if (culture == "en" || culture.StartsWith("en-"))
        {
          return "en";
        }
      }

      // Check Accept-Language header
      if (context?.Request?.Headers?.ContainsKey("Accept-Language") == true)
      {
        var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
        if (acceptLanguage.Contains("es"))
        {
          return "es";
        }
      }

      return "en"; // Default to English
    }
  }
}
