using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using _4080capstone.Models;
using System.Diagnostics;
using DynamicData;
using System.Globalization;

namespace _4080capstone.ViewModels
{
    public class KeyRingViewModel
    {
        public static ObservableCollection<PgpKeyInfo> PgpKeys { get; private set; } = new ObservableCollection<PgpKeyInfo>();
        public static ObservableCollection<PgpKeyInfo> PrivatePgpKeys => new(PgpKeys.Where(k => k.KeyType == "Private"));
        public static ObservableCollection<PgpKeyInfo> PublicPgpKeys => new(PgpKeys.Where(k => k.KeyType == "Public"));

        public KeyRingViewModel() {
            InitializeKeyCollection();
        }

        public static void InitializeKeyCollection()
        {
            if (PgpKeys.Count == 0)
            {
                string path = "keys";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var keyFiles = Directory.GetFiles("keys", "*.asc");
                foreach (var file in keyFiles)
                {
                    try { ParseAndAddKey(file); }
                    catch (Exception ex) { Console.WriteLine($"Failed to parse {file}: {ex.Message}"); }
                }
                SortPgpKeys(PgpKeys);
            }
        }

        public static void RefreshCollection()
        {
            PgpKeys.Clear();
            InitializeKeyCollection();
        }

        protected static void SortPgpKeys(ObservableCollection<PgpKeyInfo> coll)
        {
            string currentUser = AppState.Instance.CurrentUsername.ToString() ?? "";

            var sorted = coll
                .OrderByDescending(k => k.UserIdentity.Equals(currentUser, StringComparison.OrdinalIgnoreCase))
                .ThenBy(k => k.KeyType == "Private" ? 0 : 1)
                .ToList();

            coll.Clear();
            coll.AddRange(sorted);
        }

        public static void ParseAndAddKey(string path)
        {
            var newKey = ParseKey(path);
            if (newKey == null || !CanAddKey(path))
                return;
            PgpKeys.Add(newKey);
            SortPgpKeys(PgpKeys);
        }

        public static bool CanAddKey(string path)
        {
            var newKey = ParseKey(path);
            return (newKey != null && !PgpKeys.Any(key => newKey.KeyId.Equals(key.KeyId)
                                                && newKey.KeyType.Equals(key.KeyType)));
        }

        protected static PgpKeyInfo? ParseKey(string path)
        {
            try
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
                        var parsedKey = new PgpKeyInfo
                        {
                            KeyType = "Private",
                            UserIdentity = userId,
                            Validity = pubKey.IsRevoked() ? "Revoked" : "Valid",
                            CreationDate = pubKey.CreationTime,
                            ExpirationDate = expiration,
                            KeyId = key.KeyId.ToString("X"),
                            Path = path
                        };
                        return parsedKey;
                    }
                }
            }
            catch (PgpException e)
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

                        var parsedKey = new PgpKeyInfo
                        {
                            KeyType = "Public",
                            UserIdentity = userId,
                            Validity = key.IsRevoked() ? "Revoked" : "Valid",
                            CreationDate = key.CreationTime,
                            ExpirationDate = expiration,
                            KeyId = key.KeyId.ToString("X"),
                            Path = path
                        };
                        return parsedKey;
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
