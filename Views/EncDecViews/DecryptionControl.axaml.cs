﻿using _4080capstone.Services;
using _4080capstone.Views;
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
using _4080capstone.ViewModels;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Diagnostics;

namespace _4080capstone.Views;

public partial class DecryptionControl : UserControl
{
    private readonly AppState appState = AppState.Instance;

    public DecryptionControl()
    {
        InitializeComponent();
        decryptionMethod.ItemsSource = appState.EncryptionOptions;
        decryptionMethod.SelectionChanged += DecryptionMethod_SelectionChanged;
        DecryptionMethod_SelectionChanged(null, null);

        // keys
        DataContext = new KeyRingViewModel();
    }

    private void DecryptionMethod_SelectionChanged(object? sender, EventArgs? e)
    {
        bool pgpSelected = (decryptionMethod.SelectedItem.Equals("OpenPGP"));
        if (pgpSelected)
            SavedDecryptionKeys.SelectedIndex = 0;
        SavedDecryptionKeys.IsVisible = pgpSelected;
        privateKeyPassword.IsVisible = pgpSelected;
        txtDecryptKey.IsVisible = !pgpSelected;
    }

    private async void btnBrowseDecryptFile_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this) as Window;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file to decrypt",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            txtDecryptFilePath.Text = files[0].TryGetLocalPath();
        }
    }

    private async void btnDecrypt_Click(object? sender, RoutedEventArgs e)
    {
        string chosenFileName;
        IMsBox<ButtonResult> box;
        try
        {
            if (!File.Exists(txtDecryptFilePath.Text))
            {
                box = MessageBoxManager
                  .GetMessageBoxStandard("Input Error", $"File '{txtDecryptFilePath.Text}' not found.",
                            ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                return;
            }

            string? method = decryptionMethod.SelectedItem?.ToString();
            string? userEnteredKey = txtDecryptKey.Text?.Trim();
            if (decryptionMethod.SelectedItem != "OpenPGP" && string.IsNullOrWhiteSpace(userEnteredKey))
            {
                box = MessageBoxManager
                  .GetMessageBoxStandard("Input Error", "Please enter the decryption key.",
                            ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                return;
            }

            string filePath = txtDecryptFilePath.Text;
            string extension = Path.GetExtension(filePath).ToLower();
            string fileNameBase = Path.GetFileNameWithoutExtension(filePath);

            // ✅ Check if extension matches selected method
            bool extMatch = (method == "Caesar" && extension == ".cip") ||
                            (method == "XOR" && extension == ".xor") ||
                            (method == "AES" && extension == ".aes") ||
                            (method == "OpenPGP" && extension == ".pgp");

            if (!extMatch)
            {
                box = MessageBoxManager
                  .GetMessageBoxStandard("Input Error", $"Error: Selected method '{method}' does not match file extension '{extension}'.",
                            ButtonEnum.Ok);
                await box.ShowAsync();
                return;
            }

            var topLevel = TopLevel.GetTopLevel(this) as Window;
            IStorageFile? file;
            if (method == "OpenPGP")
            {
                file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Decrypted File",
                    SuggestedFileName = fileNameBase,
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("Original format") { Patterns = new[] { "*.*" }  }, // we can't get this until we decrypt :(
                        new FilePickerFileType("Normal text file") { Patterns = new[] { "*.txt*" }  },
                        new FilePickerFileType("All Files") { Patterns = new[] { "*.*" }}
                    }
                });
            } else {
                file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Decrypted File",
                    SuggestedFileName = fileNameBase,
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("Normal text file") { Patterns = new[] { "*.txt*" }  },
                        new FilePickerFileType("All Files") { Patterns = new[] { "*.*" }}
                    }
                });
            }

            if (file == null || file.Name == null)
            {
                box = MessageBoxManager
                            .GetMessageBoxStandard("Cancelled", $"Decryption cancelled.",
                            ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                return;
            }
            else
            {
                chosenFileName = file.Name;
            }

            string fullPath = txtDecryptFilePath.Text;
            string fullSaveDir = Path.GetDirectoryName(file.TryGetLocalPath());

            if (method == "AES")
            {
                byte[] allBytes = File.ReadAllBytes(filePath);
                int firstNewline = Array.IndexOf(allBytes, (byte)'\n');
                int secondNewline = Array.IndexOf(allBytes, (byte)'\n', firstNewline + 1);
                if (firstNewline < 0 || secondNewline < 0)
                    throw new Exception("Invalid AES file format.");

                string methodRead = Encoding.UTF8.GetString(allBytes[..firstNewline]).Trim();
                Debug.WriteLine(methodRead);
                string originalExt = Encoding.UTF8.GetString(allBytes[(firstNewline + 1)..secondNewline]).Trim();
                byte[] encryptedBytes;
                byte[] decryptedBytes;
                try
                {
                    byte[] encryptedSection = allBytes[(secondNewline + 1)..];
                    string base64 = Encoding.UTF8.GetString(encryptedSection).Trim();
                    encryptedBytes = Convert.FromBase64String(base64);
                    decryptedBytes = Encryption.AESDecryptBytes(encryptedBytes, userEnteredKey);
                } catch (FormatException) { 
                    encryptedBytes = allBytes[(secondNewline + 1)..];
                    decryptedBytes = Encryption.AESDecryptBytes(encryptedBytes, userEnteredKey);
                    fullPath = Path.Combine(fullSaveDir, $"{fileNameBase}{originalExt}");
                    File.WriteAllBytes(fullPath, decryptedBytes);
                }
                fullPath = Path.Combine(fullSaveDir, $"{chosenFileName}{originalExt}");
                File.WriteAllBytes(fullPath, decryptedBytes);
            } else if (method == "OpenPGP")
            {
                PgpKeyInfo keyInfo = (PgpKeyInfo)SavedDecryptionKeys.SelectedItem;
                string privateKey = File.ReadAllText(keyInfo.Path);
                fullPath = Path.Combine(fullSaveDir, $"{chosenFileName}");
                bool useOrigFileType = string.IsNullOrWhiteSpace(Path.GetExtension(fullPath));
                await Encryption.PGPDecryptFile(filePath, fullPath, privateKey, privateKeyPassword.Text ?? "", useOrigFileType);
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < 3)
                    throw new Exception("File format is invalid or incomplete.");

                string fileMethod = lines[0].Trim();
                string originalExt = lines[1].Trim();
                string encryptedText = string.Join("\n", lines.Skip(2));

                if (fileMethod != method)
                {
                    box = MessageBoxManager
                            .GetMessageBoxStandard("", $"Error: File contents indicate a different encryption method than selected.",
                            ButtonEnum.Ok);
                    await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                    return;
                }

                string output = method switch
                {
                    "Caesar" => Encryption.CaesarDecrypt(encryptedText, int.Parse(userEnteredKey)),
                    "XOR" => Encryption.XorDecrypt(encryptedText, (char)(int.Parse(userEnteredKey) % 256)),
                    _ => throw new Exception("Unknown method.")
                };
                fullPath = Path.Combine(fullSaveDir, $"{chosenFileName}{originalExt}");
                File.WriteAllText(fullPath, output);
            }

            box = MessageBoxManager
                    .GetMessageBoxStandard("Success", $"Decrypted file saved successfully.", 
                    ButtonEnum.Ok);
            await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("checksum mismatch", StringComparison.OrdinalIgnoreCase))
            {
                box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"Wrong private key password.",
                        ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
            }
            else
            {
                box = MessageBoxManager
                        .GetMessageBoxStandard("Error", $"Decryption error: {ex.Message}",
                        ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
            }
        }
    }

}