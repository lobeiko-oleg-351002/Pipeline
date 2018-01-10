using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class Sha1
    {
        public static string Encrypt(string input)
        {
            byte[] hash;
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

            }
            var sb = new StringBuilder();
            foreach (byte b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}
