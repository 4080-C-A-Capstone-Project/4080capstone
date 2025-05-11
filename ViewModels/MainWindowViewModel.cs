using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _selectedView;
        public event PropertyChangedEventHandler? PropertyChanged;

        public object SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                OnPropertyChanged(nameof(SelectedView));
            }
        }

        public MainWindowViewModel()
        {
            SelectedView = new Views.EncDecView();
            _selectedView = SelectedView;
        }

        public void ShowUserView() => SelectedView = new Views.UserSettingsView();
        public void ShowEncryptionView() => SelectedView = new Views.EncDecView();
        public void ShowKeysView() => SelectedView = new Views.KeysDirectoryView();

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
