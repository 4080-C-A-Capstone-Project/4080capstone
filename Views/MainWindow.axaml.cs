using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using System.Diagnostics;
using System.Text;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using _4080capstone.Services;
using _4080capstone.Models;
using _4080capstone.ViewModels;
using System.ComponentModel;

namespace _4080capstone.Views;

public partial class MainWindow : Window
{
    private readonly AppState appState = AppState.Instance;
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        LoadLastUserFromFile();
        DataContext = new MainWindowViewModel();
        AppState.Instance.PropertyChanged += OnUsernameChange;
        KeyRingViewModel.InitializeKeyCollection();
    }

    private void ShowUserView(object sender, RoutedEventArgs e) => ViewModel.ShowUserView();
    private void ShowEncryptionView(object sender, RoutedEventArgs e) => ViewModel.ShowEncryptionView();
    private void ShowKeysView(object sender, RoutedEventArgs e) => ViewModel.ShowKeysView();

    private void OnUsernameChange(object? sender, PropertyChangedEventArgs e) => UpdateWelcomeText();
    private void UpdateWelcomeText() => WelcomeText.Text = $"Welcome, {AppState.Instance.CurrentUsername}!";

    private void LoadLastUserFromFile()
    {
        string path = "keys.aes";
        if (!File.Exists(path))
            return;

        var lines = File.ReadAllLines(path);
        if (lines.Length == 0)
            return;

        // Get the last username entry (assumes all lines are: Username - Method - Key)
        string lastUsername = lines.Last().Split(" - ")[0];
        appState.CurrentUsername = lastUsername;
        appState.userKeys.Clear();

        foreach (var line in lines)
        {
            var parts = line.Split(" - ");
            if (parts.Length == 3 && parts[0] == appState.CurrentUsername)
            {
                string method = parts[1].Trim();
                string key = parts[2].Trim();
                appState.userKeys[method] = key;
            }
        }

        UpdateWelcomeText();
    }

}