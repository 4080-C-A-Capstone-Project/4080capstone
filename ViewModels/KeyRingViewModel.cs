using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using _4080capstone.Services;
using _4080capstone.Models;
using System.Diagnostics;

namespace _4080capstone.ViewModels
{
    public class KeyRingViewModel
    {
        public static ObservableCollection<PgpKeyInfo> PgpKeys { get; } = new ObservableCollection<PgpKeyInfo>();
        
        public KeyRingViewModel() { }

        public static void InitializeKeyCollection()
        {
            var keyFiles = Directory.GetFiles("keys", "*.asc");
            foreach (var file in keyFiles)
            {
                try { ParseAndAddKey(file); }
                catch (Exception ex) { Console.WriteLine($"Failed to parse {file}: {ex.Message}"); }
            }
        }

        // 1. user's private keys (prove authorship/read stuff meant for you)
        // 2. user's public keys (encrypted to keep to yourself)
        // 3. others' public keys (verify authenticity of other person's message)
        // 4. others' private keys (you're not really supposed to have these...)
        protected static void SortPgpKeys()
        {

        }

        public static void ParseAndAddKey(string path)
        {
            var newKey = ParseKey(path);
            if (CanAddKey(path))
                PgpKeys.Add(newKey);
        }

        public static bool CanAddKey(string path)
        {
            var newKey = ParseKey(path);
            return (newKey != null && !PgpKeys.Any(key => newKey.KeyId.Equals(key.KeyId)
                                                && newKey.KeyType.Equals(key.KeyType)));
        }

        protected static PgpKeyInfo? ParseKey(string path)
        {
            // First try to read as public key
            try
            {
                var keyContent = File.ReadAllText(path);
                if (keyContent.Contains("PRIVATE KEY BLOCK"))
                {
                    using var privateKeyStream = File.OpenRead(path);
                    using var privateDecoder = PgpUtilities.GetDecoderStream(privateKeyStream);
                    var secretKeyRingBundle = new PgpSecretKeyRingBundle(privateDecoder);

                    foreach (PgpSecretKeyRing keyRing in secretKeyRingBundle.GetKeyRings())
                    {
                        var key = keyRing.GetSecretKeys().Cast<PgpSecretKey>().FirstOrDefault(k => k.IsMasterKey)
                                ?? keyRing.GetSecretKeys().Cast<PgpSecretKey>().FirstOrDefault();
                        if (key != null)
                        {
                            var pubKey = key.PublicKey;
                            var userId = pubKey.GetUserIds().Cast<string>().FirstOrDefault() ?? "Unknown";
                            var expiration = pubKey.GetValidSeconds() == 0
                                ? DateTime.MaxValue
                                : pubKey.CreationTime.AddSeconds(pubKey.GetValidSeconds());
                            return new PgpKeyInfo
                            {
                                KeyType = "Private",
                                Identity = userId,
                                Validity = pubKey.IsRevoked() ? "Revoked" : "Valid",
                                CreationDate = pubKey.CreationTime,
                                ExpirationDate = expiration,
                                KeyId = pubKey.KeyId.ToString("X"),
                                Path = path
                            };
                        }
                    }
                }
                else if (keyContent.Contains("PUBLIC KEY BLOCK"))
                {
                    using var publicKeyStream = File.OpenRead(path);
                    using var publicDecoder = PgpUtilities.GetDecoderStream(publicKeyStream);
                    var publicKeyRingBundle = new PgpPublicKeyRingBundle(publicDecoder);

                    foreach (PgpPublicKeyRing keyRing in publicKeyRingBundle.GetKeyRings())
                    {
                        var key = keyRing.GetPublicKeys().Cast<PgpPublicKey>().FirstOrDefault(k => k.IsMasterKey);
                        if (key != null)
                        {
                            var userId = key.GetUserIds().Cast<string>().FirstOrDefault() ?? "Unknown";
                            var expiration = key.GetValidSeconds() == 0
                                ? DateTime.MaxValue
                                : key.CreationTime.AddSeconds(key.GetValidSeconds());

                            return new PgpKeyInfo
                            {
                                KeyType = "Public",
                                Identity = userId,
                                Validity = key.IsRevoked() ? "Revoked" : "Valid",
                                CreationDate = key.CreationTime,
                                ExpirationDate = expiration,
                                KeyId = key.KeyId.ToString("X"),
                                Path = path
                            };
                        }
                    }
                } 
                
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to parse key {path}: {e.Message}");
            }
            return null;
        }
    }


}
