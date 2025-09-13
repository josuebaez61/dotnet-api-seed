namespace CleanArchitecture.Application.Common.Interfaces
{
  public interface ICleanupService
  {
    Task CleanupExpiredCodesAsync();
  }
}
