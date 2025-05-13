using _4080capstone.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Text;
using _4080capstone.Models;
using System.Diagnostics;
using _4080capstone.ViewModels;

namespace _4080capstone.Views;

public partial class EncryptionControl : UserControl
{
    private readonly AppState appState = AppState.Instance;

    public EncryptionControl()
    {
        InitializeComponent();

        encryptionMethod.ItemsSource = appState.EncryptionOptions;
        
        rbText.IsCheckedChanged += RbInputType_CheckedChanged;
        rbFile.IsCheckedChanged += RbInputType_CheckedChanged;
        RbInputType_CheckedChanged(null, null);

        encryptionMethod.SelectionChanged += EncryptionMethod_SelectionChanged;
        EncryptionMethod_SelectionChanged(null, null);

        // keys
        DataContext = new KeyRingViewModel();
    }

    private void EncryptionMethod_SelectionChanged(object? sender, EventArgs? e)
    {
        bool pgpSelected = (encryptionMethod.SelectedItem.Equals("OpenPGP"));
        if (pgpSelected)
            SavedEncryptionKeys.SelectedIndex = 0;

        lblKeyInput.IsVisible = pgpSelected;
        SavedEncryptionKeys.IsVisible = pgpSelected;

        lblGeneratedKey.IsVisible = !pgpSelected;
        txtKey.IsVisible = !pgpSelected;
    }

    private void RbInputType_CheckedChanged(object? sender, EventArgs? e)
    {
        bool isText = rbText.IsChecked.GetValueOrDefault();

        // Show/hide text input
        btnOpenTextEditor.IsVisible = isText;
        lblTextInput.IsVisible = isText;

        // Show/hide file input
        txtFilePath.IsVisible = !isText;
        btnBrowseFile.IsVisible = !isText;
        lblFileInput.IsVisible = !isText;
    }

    private async void BtnOpenTextEditor_Click(object? sender, RoutedEventArgs e)
    {
        var textForm = new TextInputWindow();
        string? input = await textForm.ShowDialogAsync(TopLevel.GetTopLevel(this) as Window, appState.SavedTextInput);

        if (string.IsNullOrWhiteSpace(input)) return;

        var box = MessageBoxManager
                  .GetMessageBoxStandard("Success", "Saved text.", ButtonEnum.Ok);
        var result = await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);


        appState.SavedTextInput = input.Trim();
    }

    private async void btnBrowseFile_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        // TODO: Support multiple file encryption?
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file to encrypt",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            txtFilePath.Text = files[0].TryGetLocalPath();
        }
    }

    private async void btnEncrypt_Click(object? sender, RoutedEventArgs e)
    {
        string input = "";
        string method = encryptionMethod.SelectedItem.ToString();
        bool isFileInput = rbFile.IsChecked.GetValueOrDefault();
        string keyToUse = "";
        string output = "";
        IMsBox<ButtonResult> box;

        try
        {
            string originalName = rbFile.IsChecked.GetValueOrDefault() ? Path.GetFileNameWithoutExtension(txtFilePath.Text)! : "EncryptedText";
            string originalExt = rbFile.IsChecked.GetValueOrDefault() ? Path.GetExtension(txtFilePath.Text)! : ".txt";

            string methodExt = method switch
            {
                "Caesar" => ".cip",
                "XOR" => ".xor",
                "AES" => ".aes",
                "OpenPGP" => ".pgp",
                _ => ".enc"
            };

            string finalFileName = $"{originalName}{methodExt}";

            string fullPath;
            var topLevel = TopLevel.GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Encrypted File",
                SuggestedFileName = originalName,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType(method) { Patterns = new[] { $"*{methodExt}" } }
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

            // Try to get the local path (works on desktop platforms)
            fullPath = file.TryGetLocalPath();

            if (!isFileInput) // Text input
            {
                input = appState.SavedTextInput;
                if (string.IsNullOrWhiteSpace(input))
                {
                    box = MessageBoxManager
                    .GetMessageBoxStandard("Input Error", "No text input provided.",
                        ButtonEnum.Ok);
                    await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                    return;
                }
            }

            switch (method)
            {
                case "Caesar":
                    keyToUse = KeyGenerator.GenerateFinalCaesarKey();
                    txtKey.Text = keyToUse;

                    if (isFileInput)
                    {
                        string path = txtFilePath.Text;
                        if (Path.GetExtension(path).ToLower() != ".txt")
                            throw new Exception("Caesar only supports .txt files.");
                        input = File.ReadAllText(path);
                    }
                    // else throw new Exception("Please select input type.");

                    output = Encryption.CaesarEncrypt(input, int.Parse(keyToUse));
                    File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    break;

                case "XOR":
                    keyToUse = KeyGenerator.GenerateFinalXorKey();
                    txtKey.Text = keyToUse;

                    if (isFileInput)
                    {
                        string path = txtFilePath.Text;
                        if (Path.GetExtension(path).ToLower() != ".txt")
                            throw new Exception("XOR only supports .txt files.");
                        input = File.ReadAllText(path);
                    }
                    // else throw new Exception("Please select input type.");

                    output = Encryption.XorEncrypt(input, (char)(int.Parse(keyToUse) % 256));
                    File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    break;

                case "AES":
                    keyToUse = KeyGenerator.GenerateFinalAesKey();
                    txtKey.Text = keyToUse;

                    if (!isFileInput) // text input
                    {
                        output = Encryption.AESEncrypt(input, keyToUse);
                        File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    }
                    else if (isFileInput)
                    {
                        byte[] rawBytes = File.ReadAllBytes(txtFilePath.Text);
                        byte[] encryptedBytes = Encryption.AESEncryptBytes(rawBytes, keyToUse);

                        using var fs = new FileStream(fullPath, FileMode.Create);
                        using var writer = new BinaryWriter(fs);
                        writer.Write(Encoding.UTF8.GetBytes($"{method}\n{originalExt}\n"));
                        writer.Write(encryptedBytes);
                    }
                    else throw new Exception("Please select input type.");
                    break;
                case "OpenPGP":
                    PgpKeyInfo keyInfo = (PgpKeyInfo)SavedEncryptionKeys.SelectedItem;
                    string publicKey = File.ReadAllText(keyInfo.Path);
                    if (!isFileInput)
                    {
                        output = await Encryption.PGPEncryptText(input, publicKey);
                        File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    }
                    else if (isFileInput)
                    {
                        Encryption.PGPEncryptFileStream(txtFilePath.Text, fullPath, publicKey);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            box = MessageBoxManager
                .GetMessageBoxStandard("Error", $"Encryption error: {ex.Message}",
                        ButtonEnum.Ok);
            await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
            return;
        }
        box = MessageBoxManager
              .GetMessageBoxStandard("Success", $"Encrypted file saved successfully.",
                        ButtonEnum.Ok);
        await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
    }
}