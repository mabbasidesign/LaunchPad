using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LaunchPad.Validation
{
    /// <summary>
    /// Custom validation attribute for password complexity requirements.
    /// Ensures passwords meet security standards.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        private const int MinLength = 8;
        private const int MaxLength = 128;

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return false; // Required validation should handle this
            }

            var password = value.ToString()!;

            // Check length
            if (password.Length < MinLength || password.Length > MaxLength)
            {
                ErrorMessage = $"Password must be between {MinLength} and {MaxLength} characters.";
                return false;
            }

            // Check for lowercase letters
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ErrorMessage = "Password must contain at least one lowercase letter (a-z).";
                return false;
            }

            // Check for uppercase letters
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ErrorMessage = "Password must contain at least one uppercase letter (A-Z).";
                return false;
            }

            // Check for digits
            if (!Regex.IsMatch(password, @"\d"))
            {
                ErrorMessage = "Password must contain at least one digit (0-9).";
                return false;
            }

            // Check for special characters
            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
            {
                ErrorMessage = "Password must contain at least one special character (@, $, !, %, *, ?, or &).";
                return false;
            }

            // Check for spaces
            if (password.Contains(" "))
            {
                ErrorMessage = "Password cannot contain spaces.";
                return false;
            }

            // Check for consecutive identical characters
            if (HasConsecutiveIdenticalChars(password, 3))
            {
                ErrorMessage = "Password cannot contain more than 2 consecutive identical characters.";
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"{name} does not meet complexity requirements.";
        }

        /// <summary>
        /// Checks if password has more than the specified count of consecutive identical characters.
        /// </summary>
        private static bool HasConsecutiveIdenticalChars(string password, int maxConsecutive)
        {
            for (int i = 0; i < password.Length - (maxConsecutive - 1); i++)
            {
                bool allSame = true;
                for (int j = 1; j < maxConsecutive; j++)
                {
                    if (password[i] != password[i + j])
                    {
                        allSame = false;
                        break;
                    }
                }
                if (allSame)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
