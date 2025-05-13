using _4080capstone.Models;
using _4080capstone.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using _4080capstone.ViewModels;

namespace _4080capstone.Views;

public partial class UserSettingsView : UserControl
{
    private readonly AppState appState = AppState.Instance;
    public UserSettingsView()
    {
        InitializeComponent();
        DataContext = appState;
    }


    private async void SetUsername_Click(object? sender, RoutedEventArgs e)
    {
        var usernameWindow = new UsernameSetWindow();

        string? input = await usernameWindow.ShowDialogAsync(TopLevel.GetTopLevel(this) as Window, (string)appState.CurrentUsername);

        if (string.IsNullOrWhiteSpace(input)) return;
        
        appState.CurrentUsername = input.Trim();
        appState.userKeys.Clear(); // reset

        KeyGenerator.GenerateKeys();

        var box = MessageBoxManager
            .GetMessageBoxStandard("Success", $"Keys saved to keys.aes for user: {appState.CurrentUsername}",
                ButtonEnum.Ok);
        var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
    }

    private void GenerateKeys(object? sender, RoutedEventArgs e)
    {
        KeyGenerator.GenerateKeys();
    }

}