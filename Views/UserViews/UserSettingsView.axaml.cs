using _4080capstone.Models;
using _4080capstone.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace _4080capstone.Views;

public partial class UserSettingsView : UserControl
{
    private readonly AppState appState = AppState.Instance;
    public UserSettingsView()
    {
        InitializeComponent();
    }


    private async void SetUsername_Click(object? sender, RoutedEventArgs e)
    {
        var usernameWindow = new UsernameSetWindow();

        string? input = await usernameWindow.ShowDialogAsync(TopLevel.GetTopLevel(this) as Window, (string)appState.CurrentUsername);

        if (string.IsNullOrWhiteSpace(input)) return;

        appState.CurrentUsername = input.Trim();
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