using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Zune.DB
{
    public static class Helpers
    {

        private static byte[] GetSha256(string content)
        {
            using var hasher = SHA256.Create();
            var byteArrayResultOfRawData = Encoding.UTF8.GetBytes(content);

            return hasher.ComputeHash(byteArrayResultOfRawData);
        }
        
        public static Guid GenerateGuid(string content)
        {
            var result = GetSha256(content);
            var guidBase = new byte[16];
            for(int i = 0;i < 16; i++)
            {
                guidBase[i] = result[i];
            }
            return new(guidBase);
        }

        public static string Hash(string str)
        {
            var result = GetSha256(str);

            string[] hashStr = new string[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                hashStr[i] = result[i].ToString("X2");
            }

            return string.Join(string.Empty, hashStr).ToUpperInvariant();
        }
    }
}
