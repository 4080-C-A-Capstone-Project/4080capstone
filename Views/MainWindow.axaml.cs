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

namespace _4080capstone.Views;

public partial class MainWindow : Window
{
    private readonly AppState appState = AppState.Instance;

    public MainWindow()
    {
        InitializeComponent();
        LoadLastUserFromFile();
    }

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

        //MessageBoxBox.Show($"Loaded last user: {currentUsername}", "Welcome");
        userMenu.Header = $"Current User: {appState.CurrentUsername}";
    }

    private async void SetUsername_Click(object? sender, RoutedEventArgs e)
    {
        var usernameWindow = new UsernameSetWindow();

        string? input = await usernameWindow.ShowDialogAsync(this, appState.CurrentUsername);

        if (string.IsNullOrWhiteSpace(input)) return;

        appState.CurrentUsername = input.Trim();
        userMenu.Header = $"Current User: {appState.CurrentUsername}";
        appState.userKeys.Clear(); // reset

        string caesarKey = KeyGenerator.GenerateNumericKey(4);
        string xorKey = KeyGenerator.GenerateNumericKey(4);
        string aesUserKey = KeyGenerator.GenerateAlphaNumKey(8);

        appState.userKeys["Caesar"] = caesarKey;
        appState.userKeys["XOR"] = xorKey;
        appState.userKeys["AES"] = aesUserKey;

        var lines = appState.userKeys.Select(kvp => $"{appState.CurrentUsername} - {kvp.Key} - {kvp.Value}");
        File.WriteAllLines("keys.aes", lines);

        var box = MessageBoxManager
            .GetMessageBoxStandard("Success", $"Keys saved to keys.aes for user: {appState.CurrentUsername}",
                ButtonEnum.Ok);
        var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
    }

}