namespace CleanArchitecture.API.Common
{
  /// <summary>
  /// Constants for environment names and validation
  /// </summary>
  public static class EnvironmentConstants
  {
    /// <summary>
    /// Development environment name
    /// </summary>
    public const string Development = "Development";

    /// <summary>
    /// Staging environment name
    /// </summary>
    public const string Staging = "Staging";

    /// <summary>
    /// Production environment name
    /// </summary>
    public const string Production = "Production";

    /// <summary>
    /// List of all allowed environments
    /// </summary>
    public static readonly string[] AllowedEnvironments = { Development, Staging, Production };

    /// <summary>
    /// Validates if the given environment is allowed
    /// </summary>
    /// <param name="environment">Environment name to validate</param>
    /// <returns>True if environment is allowed, false otherwise</returns>
    public static bool IsValidEnvironment(string? environment)
    {
      if (string.IsNullOrWhiteSpace(environment))
        return false;

      return AllowedEnvironments.Contains(environment, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets a list of allowed environments as a formatted string
    /// </summary>
    /// <returns>Comma-separated list of allowed environments</returns>
    public static string GetAllowedEnvironmentsString()
    {
      return string.Join(", ", AllowedEnvironments);
    }

    /// <summary>
    /// Throws an exception if the environment is not valid
    /// </summary>
    /// <param name="environment">Environment to validate</param>
    /// <exception cref="InvalidOperationException">Thrown when environment is not valid</exception>
    public static void ValidateEnvironment(string? environment)
    {
      if (!IsValidEnvironment(environment))
      {
        var allowedEnvironments = GetAllowedEnvironmentsString();
        throw new InvalidOperationException(
            $"❌ Invalid environment: '{environment}'. " +
            $"Only the following environments are allowed: {allowedEnvironments}. " +
            $"Please set ASPNETCORE_ENVIRONMENT to one of these values."
        );
      }
    }
  }
}
