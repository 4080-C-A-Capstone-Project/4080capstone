using System;
using System.Collections.Generic;
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

        public Dictionary<string, string> userKeys = new();
        public string SavedTextInput { get; set; } = "";
        public string[] EncryptionOptions = { "Caesar", "XOR", "AES" }; // maybe make dictionary or tuple to store descriptions

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
