using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VT.Common.Utils
{
    public class PasswordUtil
    {
        public static string GenerateSalt()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
        }

        public static string CreatedHashedPassword(string password, string salt)
        {
            var combined = password + salt;
            return CreateHash(combined);
        }

        private static string CreateHash(string s)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(s))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static IEnumerable<byte> GetHash(string inputString)
        {
            var algorithm = MD5.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}
