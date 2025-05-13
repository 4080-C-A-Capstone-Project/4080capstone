using _4080capstone.Models;
using _4080capstone.ViewModels;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.Services
{
    public static class KeyGenerator
    {
        public static async void GenerateKeyPair(string identity, string passphrase)
        {
            PGP pgp = new PGP();

            // Generate keypair in memory
            using var publicKeyStream = new MemoryStream();
            using var privateKeyStream = new MemoryStream();
            await pgp.GenerateKeyAsync(
                publicKeyStream, privateKeyStream,
                username: identity, // E-mail or username
                password: passphrase
            );
            publicKeyStream.Position = 0;

            // Extract key ID from generated key
            PgpPublicKeyRingBundle pubRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(publicKeyStream));
            PgpPublicKey pubKey = pubRing.GetKeyRings().Cast<PgpPublicKeyRing>().First().GetPublicKeys().Cast<PgpPublicKey>().First(k => k.IsEncryptionKey);

            string path = "keys";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            long keyId = pubKey.KeyId;
            string filename = $"{keyId:X}.asc";
            publicKeyStream.Position = 0;
            File.WriteAllBytes($"keys/{filename}", publicKeyStream.ToArray());
            privateKeyStream.Position = 0;
            File.WriteAllBytes($"keys/{filename.Replace(".asc", "_private.asc")}", privateKeyStream.ToArray());

            KeyRingViewModel.ParseAndAddKey($"keys/{filename}");
            KeyRingViewModel.ParseAndAddKey($"keys/{filename.Replace(".asc", "_private.asc")}");
        }

        public static void GenerateKeys()
        {
            AppState appState = AppState.Instance;
            if (!string.IsNullOrWhiteSpace(appState.CurrentUsername.ToString()))
            {
                appState.userKeys.Clear(); // reset

                string caesarKey = KeyGenerator.GenerateNumericKey(4);
                string xorKey = KeyGenerator.GenerateNumericKey(4);
                string aesUserKey = KeyGenerator.GenerateAlphaNumKey(8);
                appState.userKeys["Caesar"] = caesarKey;
                appState.userKeys["XOR"] = xorKey;
                appState.userKeys["AES"] = aesUserKey;
                var lines = appState.userKeys.Select(kvp => $"{appState.CurrentUsername} - {kvp.Key} - {kvp.Value}");
                File.WriteAllLines("keys.aes", lines);

                appState.UpdateKeyCollection();
            }
        }
        public static string GenerateNumericKey(int digits)
        {
            var rnd = new Random();
            return string.Concat(Enumerable.Range(0, digits).Select(_ => rnd.Next(10).ToString()));
        }

        public static string GenerateAlphaNumKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rnd = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }

        public static string GenerateFinalCaesarKey()
        {
            int shift = new Random().Next(1, 26); // shift: 1–25
            int userNum = int.Parse(AppState.Instance.userKeys["Caesar"]);
            return (shift + userNum).ToString();
        }

        public static string GenerateFinalXorKey()
        {
            int shift = new Random().Next(1, 26);
            int userNum = int.Parse(AppState.Instance.userKeys["XOR"]);
            return (shift + userNum).ToString();
        }

        public static string GenerateFinalAesKey()
        {
            string userKey = AppState.Instance.userKeys["AES"];
            string base24 = GenerateAlphaNumKey(24);
            var rnd = new Random();
            int insertPos = rnd.Next(0, 25); // insert before pos
            return base24.Substring(0, insertPos) + userKey + base24.Substring(insertPos);
        }

    }
}