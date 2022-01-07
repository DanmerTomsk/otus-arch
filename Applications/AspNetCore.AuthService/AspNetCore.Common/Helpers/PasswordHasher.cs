using System;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.Common.Helpers
{
    public static class PasswordHasher
    {
        public static int GetHash(string input)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            var result = 0;
            for (int i = 0; i < hash.Length; i++)
            {
                result += hash[i] * (i % 7) ^ 157;
            }

            return result;
        }

        public static bool VerifyHash(string input, int hash)
        {
            var hashOfInput = GetHash(input);

            return hashOfInput == hash;
        }
    }
}
