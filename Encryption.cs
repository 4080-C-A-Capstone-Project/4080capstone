using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptorApp
{
    public static class Encryption
    {
        public static string CaesarEncrypt(string text, int shift = 3)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
                sb.Append((char)(c + shift));
            return sb.ToString();
        }

        public static string CaesarDecrypt(string text, int shift = 3)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
                sb.Append((char)(c - shift));
            return sb.ToString();
        }

        public static string XorEncrypt(string text, char key = 'K')
        {
            var result = new StringBuilder();
            foreach (char c in text)
                result.Append((char)(c ^ key));
            return result.ToString();
        }

        public static string XorDecrypt(string text, char key = 'K') => XorEncrypt(text, key);

        public static string AESEncrypt(string text, string key)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16];
            var encryptor = aes.CreateEncryptor();
            byte[] input = Encoding.UTF8.GetBytes(text);
            byte[] result = encryptor.TransformFinalBlock(input, 0, input.Length);
            return Convert.ToBase64String(result);
        }

        public static string AESDecrypt(string base64Text, string key)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16];
            var decryptor = aes.CreateDecryptor();
            byte[] input = Convert.FromBase64String(base64Text);
            byte[] result = decryptor.TransformFinalBlock(input, 0, input.Length);
            return Encoding.UTF8.GetString(result);
        }
    }
}