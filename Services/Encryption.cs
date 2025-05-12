using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Org.BouncyCastle.Security;
using PgpCore;
using PgpCore.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _4080capstone.Services
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

        public static byte[] AESEncryptBytes(byte[] data, string key)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // Zero IV for simplicity; consider random for better security

            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return ms.ToArray();
        }

        public static byte[] AESDecryptBytes(byte[] encryptedData, string key)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16];

            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(encryptedData, 0, encryptedData.Length);
            Debug.WriteLine("");
            cryptoStream.FlushFinalBlock();

            return ms.ToArray();
        }

        public async static Task<string> PGPEncryptText(string text, string key)
        {
            // string publicKey = File.ReadAllText(@"C:\TEMP\Keys\public.asc");
            string encKey = key; // usually the other party's public key
            EncryptionKeys encryptionKeys = new EncryptionKeys(encKey); // For now, only one recipient

            PGP pgp = new PGP(encryptionKeys);
            string encryptedContent = await pgp.EncryptAsync(text);

            return encryptedContent;
        }

        public async static void PGPEncryptFileStream(string inputPath, string outputPath, string key)
        {
            EncryptionKeys encryptionKeys = new EncryptionKeys(key);
            PGP pgp = new PGP(encryptionKeys);

            // Reference input/output files
            using (FileStream inputFileStream = new FileStream(inputPath, FileMode.Open))
            using (Stream outputFileStream = File.Create(outputPath))
            {
                await pgp.EncryptAsync(inputFileStream, outputFileStream);
                inputFileStream.Close();
                outputFileStream.Close();
            }
        }

        public async static Task<string> PGPDecryptText(string text, string key, string? password = null)
        {
            EncryptionKeys encryptionKeys;

            if (string.IsNullOrWhiteSpace(password)) 
                encryptionKeys = new EncryptionKeys(key);
            else
                encryptionKeys = new EncryptionKeys(key, password);

            PGP pgp = new PGP(encryptionKeys);

            // Decrypt
            string decryptedContent = await pgp.DecryptAsync(text);
            return decryptedContent;
        }

        public async static Task PGPDecryptFile(string inputPath, string outputPath, string key, string? password = null)
        {
            EncryptionKeys encryptionKeys = new EncryptionKeys(key, password);

            // Reference input/output files
            FileInfo inputFile = new FileInfo(inputPath);
            FileInfo decryptedFile = new FileInfo(outputPath);

            // Decrypt
            PGP pgp = new PGP(encryptionKeys);
            await pgp.DecryptAsync(inputFile, decryptedFile);
        }
    }
}