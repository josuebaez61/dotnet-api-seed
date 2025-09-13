namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface ILocalizationService
  {
    string GetString(string key, params object[] args);
    string GetSuccessMessage(string key, params object[] args);
    string GetErrorMessage(string key, params object[] args);
    string GetValidationMessage(string key, params object[] args);
  }
}
