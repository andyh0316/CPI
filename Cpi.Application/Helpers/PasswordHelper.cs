using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Cpi.Application.Helpers
{
    public static class PasswordHelper
    {
        public static string EncodePassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password + salt);
            HashAlgorithm hash = new SHA256Managed();
            byte[] hashedBytes = hash.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
