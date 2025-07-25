﻿using System.Text.RegularExpressions;

namespace WebApi.Extension
{
    public class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Manager = "Manager";
        public const string ManagerAndUser = "Manager, User";
        public const string AdminAndManager = "Admin, Manager";
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Validates whether the given string is in a valid email format.
        /// </summary>
        /// <param name="email">Input email string</param>
        /// <returns>True if the email format is valid, otherwise false.</returns>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Simple but effective email regex
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
        }
    }
}
