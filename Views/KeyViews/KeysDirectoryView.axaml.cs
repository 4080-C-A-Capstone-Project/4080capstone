using _4080capstone.Models;
using _4080capstone.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Base;
using System.Diagnostics;

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

    private async void btnExportKey_Click(object? sender, RoutedEventArgs e)
    {
        if ((PgpKeyInfo)KeyringTable.SelectedItem is var selectedKey)
        {
            IMsBox<ButtonResult> box;
            ButtonResult result;

            var topLevel = TopLevel.GetTopLevel(this);
            try
            {
                string originalName = Path.GetFileName(selectedKey.Path);
                string originalNameNoExt = Path.GetFileNameWithoutExtension(selectedKey.Path);
                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Export As",
                    SuggestedFileName = originalNameNoExt,
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("OpenPGP Text File") { Patterns = new[] { $"*asc" } }
                    }
                });

                if (file == null)
                {
                    box = MessageBoxManager
                        .GetMessageBoxStandard("", "Encryption cancelled.",
                            ButtonEnum.Ok);
                    await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                    return;
                }
                File.Copy(selectedKey.Path, file.TryGetLocalPath() + ".asc");
            }
            catch (Exception ex)
            {
                box = MessageBoxManager
                          .GetMessageBoxStandard("Export Error", $"Error: {ex.Message}", ButtonEnum.Ok);
                result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                return;
            }
            box = MessageBoxManager
                      .GetMessageBoxStandard("Success", "Key exported successfully.", ButtonEnum.Ok);
            result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
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