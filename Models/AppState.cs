using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.Models
{
    public sealed class AppState
    {
        public static AppState Instance { get; } = new AppState();
        private AppState() { }

        public string CurrentUsername { get; set; } = "";
        public Dictionary<string, string> userKeys = new();
        public string SavedTextInput { get; set; } = "";
        public string[] EncryptionOptions = { "Caesar", "XOR", "AES" }; // maybe make dictionary or tuple to store descrptions
    }
}
