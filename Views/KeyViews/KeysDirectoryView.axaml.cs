using _4080capstone.Models;
using _4080capstone.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia.Platform.Storage;

namespace _4080capstone.Views;

public partial class KeysDirectoryView : UserControl
{
    public KeysDirectoryView()
    {
        InitializeComponent();
        DataContext = new KeyRingViewModel();
    }

    private async void btnImportKey_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a key to import",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("OpenPGP Text File") { Patterns = new[] { "*.asc" }  },
            }
        });

        if (files.Count >= 1)
        {
            string KeyFilePath = files[0].TryGetLocalPath();
            if (KeyRingViewModel.CanAddKey(KeyFilePath)) {
                try
                {
                    File.Copy(KeyFilePath, "keys/" + files[0].Name);
                    KeyRingViewModel.ParseAndAddKey(KeyFilePath);
                    var box = MessageBoxManager
                              .GetMessageBoxStandard("Success", $"Imported {files[0].Name}.", ButtonEnum.Ok);
                    var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                    return;
                }
                catch (IOException ex) when (ex is IOException)
                {
                    var box = MessageBoxManager
                              .GetMessageBoxStandard("Error", $"File named {files[0].Name} already exists in keys folder.", ButtonEnum.Ok);
                    var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                    return;
                }
            } else
            {
                var box = MessageBoxManager
                              .GetMessageBoxStandard("Error", $"Key is invalid or already exists in keys folder.", ButtonEnum.Ok);
                var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                return;
            }
        }
    }

    private async void btnCreateKeyPair_Click(object? sender, RoutedEventArgs e)
    {
        var textForm = new KeyCreationWindow();
        bool? keyMade = await textForm.ShowDialogAsync(TopLevel.GetTopLevel(this) as Window, AppState.Instance.CurrentUsername.ToString());

        if (!keyMade.GetValueOrDefault()) return;

        var box = MessageBoxManager
                  .GetMessageBoxStandard("Success", "Key pair successfully generated.", ButtonEnum.Ok);
        var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
    }
}