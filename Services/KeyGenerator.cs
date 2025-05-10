using _4080capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.Services
{

    public static class KeyGenerator
    {
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