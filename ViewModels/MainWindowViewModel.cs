using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using _4080capstone.Models;
using _4080capstone.Views;

namespace _4080capstone.ViewModels
{

    public class MainWindowViewModel : ReactiveObject
    {
        public enum Page
        {
            User,
            EncDec,
            Keys
        }

        private object _selectedView;
        public object SelectedView
        {
            get => _selectedView;
            set => this.RaiseAndSetIfChanged(ref _selectedView, value);
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        public MainWindowViewModel()
        {
            if (string.IsNullOrWhiteSpace(AppState.Instance.CurrentUsername.ToString())) { // no user yet 
                SelectedView = new UserSettingsView();
                CurrentPage = Page.User;
            } else {
                SelectedView = new EncDecView();
                CurrentPage = Page.EncDec;
            }
                
        }

        public void ShowUserView() => SelectedView = new UserSettingsView();
        public void ShowEncryptionView() => SelectedView = new EncDecView();
        public void ShowKeysView() => SelectedView = new KeysDirectoryView();

    }

}
