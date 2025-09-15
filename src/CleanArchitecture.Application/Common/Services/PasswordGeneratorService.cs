using System;
using System.Linq;
using System.Security.Cryptography;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Common.Services
{
    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        public string GenerateSecurePassword(int length = 12)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters", nameof(length));

            var allChars = LowercaseChars + UppercaseChars + DigitChars + SpecialChars;
            var password = new char[length];
            var randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Ensure at least one character from each category
            password[0] = LowercaseChars[randomBytes[0] % LowercaseChars.Length];
            password[1] = UppercaseChars[randomBytes[1] % UppercaseChars.Length];
            password[2] = DigitChars[randomBytes[2] % DigitChars.Length];
            password[3] = SpecialChars[randomBytes[3] % SpecialChars.Length];

            // Fill the rest randomly
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[randomBytes[i] % allChars.Length];
            }

            // Shuffle the password to avoid predictable patterns
            for (int i = 0; i < length; i++)
            {
                int randomIndex = randomBytes[i % randomBytes.Length] % length;
                (password[i], password[randomIndex]) = (password[randomIndex], password[i]);
            }

            return new string(password);
        }
    }
}
