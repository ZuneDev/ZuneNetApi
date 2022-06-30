using System;
using System.Security.Cryptography;
using System.Text;

namespace Zune.DB
{
    public static class Helpers
    {
        public static Guid GenerateGuid(string content)
        {
            // Using MD5 here is fine, as these GUIDs aren't used for
            // anything meant to be secure. We could use SHA256, but
            // we'd have to crush it down to 16 bytes for the GUID
            // anyway. Might as well use MD5 ¯\_(ツ)_/¯

            // Compute 128-bit (16-byte) hash
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(content));
            return new(hash);
        }

        public static string Hash(string str)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(str));

            string[] hashStr = new string[hash.Length];
            for (int i = 0; i < hash.Length; i++)
                hashStr[i] = hash[i].ToString("X2");

            return string.Join(string.Empty, hashStr).ToUpperInvariant();
        }
    }
}
