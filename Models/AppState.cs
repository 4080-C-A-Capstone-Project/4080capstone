using _4080capstone.Services;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.Models
{
    public sealed class AppState : INotifyPropertyChanged
    {
        public static AppState Instance { get; } = new AppState();
        private AppState() { }
        public string SessionInfo => (!string.IsNullOrWhiteSpace(CurrentUsername.ToString())) ? $"You are currently logged in as {CurrentUsername.ToString()}." : "You are not yet logged in.";
        public ObservableCollection<KeyEntry> KeyEntries { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        private object _currentUsername = "";
        public object CurrentUsername
        {
            get => _currentUsername;
            set
            {
                if (!Equals(_currentUsername, value))  // prevent unnecessary notifications (we only gaf abt name change)
                {
                    _currentUsername = value;
                    OnPropertyChanged(nameof(CurrentUsername));
                }
            }
        }

        public void UpdateKeyCollection()
        {
            KeyEntries.Clear();
            foreach (var kvp in userKeys)
                KeyEntries.Add(new KeyEntry { Method = kvp.Key, EncKey = kvp.Value });
        }

        public Dictionary<string, string> userKeys = new(); // session keys
        public string SavedTextInput { get; set; } = "";
        public string[] EncryptionOptions = { "Caesar", "XOR", "AES", "OpenPGP" }; // maybe make dictionary or tuple to store descriptions

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class KeyEntry() // to show in view; regular dictionary doesn't work
        {
            public string Method { get; set; }
            public string EncKey { get; set; }
        }
    }
}
