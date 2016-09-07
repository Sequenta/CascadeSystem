using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Domain
{
    public static class Password
    {
        public static string Create(string rawPassword)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(rawPassword).ToArray();
            var hash = MD5.Create().ComputeHash(passwordBytes);
            return Convert.ToBase64String(hash);
        }

        public static bool Matches(string rawPassword, string passwordHash)
        {
            return Create(rawPassword) == passwordHash;
        }
    }
}