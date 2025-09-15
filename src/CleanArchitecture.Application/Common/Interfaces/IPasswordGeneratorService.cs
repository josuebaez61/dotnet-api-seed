namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IPasswordGeneratorService
    {
        string GenerateSecurePassword(int length = 12);
    }
}
