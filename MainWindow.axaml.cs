using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using System.Diagnostics;
using System.Text;
using Avalonia.Platform.Storage;

namespace _4080capstone;

public partial class MainWindow : Window
{
    private string currentUsername = "";
    private Dictionary<string, string> userKeys = new();
    private string savedTextInput = "";

    public MainWindow()
    {
        InitializeComponent();

        // Initialize dropdowns with encryption types
        var _encryptionOptions = new[] { "Caesar", "XOR", "AES"};
        encryptionMethod.ItemsSource = _encryptionOptions;
        decryptionMethod.ItemsSource = _encryptionOptions;

        rbText.IsCheckedChanged+= RbInputType_CheckedChanged;
        rbFile.IsCheckedChanged += RbInputType_CheckedChanged;
        RbInputType_CheckedChanged(null, null);

        LoadLastUserFromFile();
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

    private void BtnOpenTextEditor_Click(object? sender, RoutedEventArgs e)
    {
        var textForm = TextInputWindow.GetInstance();
        textForm.Show();
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
        currentUsername = lastUsername;
        userKeys.Clear();

        foreach (var line in lines)
        {
            var parts = line.Split(" - ");
            if (parts.Length == 3 && parts[0] == currentUsername)
            {
                string method = parts[1].Trim();
                string key = parts[2].Trim();
                userKeys[method] = key;
            }
        }

        //MessageBoxBox.Show($"Loaded last user: {currentUsername}", "Welcome");
        userMenu.Header = $"Current User: {currentUsername}";
    }


    private void SetUsername_Click(object? sender, RoutedEventArgs e)
    {
        /*var textForm = UsernameSetWindow.GetInstance();
        textForm.Show();*/

        string input = Microsoft.VisualBasic.Interaction.InputBox("Enter a username:", "Set Username", currentUsername);
        if (string.IsNullOrWhiteSpace(input)) return;

        currentUsername = input.Trim();
        userKeys.Clear(); // reset

        string caesarKey = GenerateNumericKey(4);
        string xorKey = GenerateNumericKey(4);
        string aesUserKey = GenerateAlphaNumKey(8);

        userKeys["Caesar"] = caesarKey;
        userKeys["XOR"] = xorKey;
        userKeys["AES"] = aesUserKey;

        var lines = userKeys.Select(kvp => $"{currentUsername} - {kvp.Key} - {kvp.Value}");
        File.WriteAllLines("keys.aes", lines);
        //MessageBoxBox.Show($"Keys saved to keys.aes for user: {currentUsername}");
        userMenu.Header = $"Current User: {currentUsername}";
    }

    private string GenerateNumericKey(int digits)
    {
        var rnd = new Random();
        return string.Concat(Enumerable.Range(0, digits).Select(_ => rnd.Next(10).ToString()));
    }

    private string GenerateAlphaNumKey(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var rnd = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
    }

    private string GenerateFinalCaesarKey()
    {
        int shift = new Random().Next(1, 26); // shift: 1–25
        int userNum = int.Parse(userKeys["Caesar"]);
        return (shift + userNum).ToString();
    }

    private string GenerateFinalXorKey()
    {
        int shift = new Random().Next(1, 26);
        int userNum = int.Parse(userKeys["XOR"]);
        return (shift + userNum).ToString();
    }

    private string GenerateFinalAesKey()
    {
        string userKey = userKeys["AES"];
        string base24 = GenerateAlphaNumKey(24);
        var rnd = new Random();
        int insertPos = rnd.Next(0, 25); // insert before pos
        return base24.Substring(0, insertPos) + userKey + base24.Substring(insertPos);
    }

    private async void btnBrowseFile_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

        // TODO: Support multiple file encryption
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file to encrypt",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            txtFilePath.Text = files[0].TryGetLocalPath();
        }
    }


    private async void btnBrowseDecryptFile_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

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

    private async void btnEncrypt_Click(object? sender, RoutedEventArgs e)
    {
        string input = "";
        string method = encryptionMethod.SelectedItem.ToString();
        bool isFileInput = rbFile.IsChecked.GetValueOrDefault();
        string keyToUse = "";
        string output = "";

        try
        {
            string originalName = rbFile.IsChecked.GetValueOrDefault() ? Path.GetFileNameWithoutExtension(txtFilePath.Text)! : "EncryptedText";
            string originalExt = rbFile.IsChecked.GetValueOrDefault() ? Path.GetExtension(txtFilePath.Text)! : ".txt";

            string methodExt = method switch
            {
                "Caesar" => ".cip",
                "XOR" => ".xor",
                "AES" => ".aes",
                _ => ".enc"
            };

            string finalFileName = $"{originalName}{methodExt}";

            /*SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Encrypted File";
            saveFileDialog.FileName = originalName;
            saveFileDialog.Filter = "All Files|*.*";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                //MessageBox.Show("Encryption cancelled.");
                return;
            }*/

            string fullPath;
            var topLevel = GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Encrypted File",
                SuggestedFileName = originalName,
                FileTypeChoices = new[]
                {
                    /*new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }*/
                    new FilePickerFileType(method) { Patterns = new[] { $"*{methodExt}" } }
                }
            });
            
            // Try to get the local path (works on desktop platforms)
            fullPath = file.TryGetLocalPath();

            switch (method)
            {
                case "Caesar":
                    keyToUse = GenerateFinalCaesarKey();
                    txtKey.Text = keyToUse;

                    if (rbText.IsChecked.GetValueOrDefault())
                    {
                        input = TextInputWindow.GetInstance().UserInput.Text;
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            //MessageBoxBox.Show("No text input provided.");
                            return;
                        }
                    }

                    else if (isFileInput)
                    {
                        string path = txtFilePath.Text;
                        if (Path.GetExtension(path).ToLower() != ".txt")
                            throw new Exception("Caesar only supports .txt files.");
                        input = File.ReadAllText(path);
                    }
                    else throw new Exception("Please select input type.");

                    output = Encryption.CaesarEncrypt(input, int.Parse(keyToUse));
                    File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    break;

                case "XOR":
                    keyToUse = GenerateFinalXorKey();
                    txtKey.Text = keyToUse;

                    if (rbText.IsChecked.GetValueOrDefault())
                    {
                        input = savedTextInput;
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            //MessageBoxBox.Show("No text input provided.");
                            return;
                        }
                    }

                    else if (isFileInput)
                    {
                        string path = txtFilePath.Text;
                        if (Path.GetExtension(path).ToLower() != ".txt")
                            throw new Exception("XOR only supports .txt files.");
                        input = File.ReadAllText(path);
                    }
                    else throw new Exception("Please select input type.");

                    output = Encryption.XorEncrypt(input, (char)(int.Parse(keyToUse) % 256));
                    File.WriteAllText(fullPath, $"{method}\n{originalExt}\n{output}");
                    break;

                case "AES":
                    keyToUse = GenerateFinalAesKey();
                    txtKey.Text = keyToUse;

                    if (rbText.IsChecked.GetValueOrDefault())
                    {
                        input = TextInputWindow.GetInstance().UserInput.Text ?? "";
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
            }

            //MessageBoxBox.Show("Encrypted file saved successfully.");
        }
        catch (Exception ex)
        {
            //MessageBoxBox.Show($"Encryption error: {ex.Message}");
        }
    }

    private async void btnDecrypt_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!File.Exists(txtDecryptFilePath.Text))
            {
                //MessageBox.Show("Decryption file not found.");
                return;
            }

            string? method = encryptionMethod.SelectedItem?.ToString();
            string? userEnteredKey = txtDecryptKey.Text?.Trim();
            if (string.IsNullOrWhiteSpace(userEnteredKey))
            {
                //MessageBox.Show("Please enter the decryption key.");
                return;
            }

            string filePath = txtDecryptFilePath.Text;
            string extension = Path.GetExtension(filePath).ToLower();
            string fileNameBase = Path.GetFileNameWithoutExtension(filePath);

            // ✅ Check if extension matches selected method
            bool extMatch = (method == "Caesar" && extension == ".cip") ||
                            (method == "XOR" && extension == ".xor") ||
                            (method == "AES" && extension == ".aes");

            if (!extMatch)
            {
                //MessageBox.Show($"Error: Selected method '{method}' does not match file extension '{extension}'.", "Extension Mismatch", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                return;
            }


            var topLevel = GetTopLevel(this);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Decrypted File",
                SuggestedFileName = fileNameBase,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Normal text file") { Patterns = new[] { "*.txt*" }  },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*.*" }}
                }
            });

            /*string fullSaveDir = Path.GetDirectoryName(file);
            string fullPath;*/

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
                string originalExt = Encoding.UTF8.GetString(allBytes[(firstNewline + 1)..secondNewline]).Trim();

                byte[] encryptedBytes = allBytes[(secondNewline + 1)..];
                byte[] decryptedBytes = Encryption.AESDecryptBytes(encryptedBytes, userEnteredKey);
                fullPath = Path.Combine(fullSaveDir, $"{fileNameBase}{originalExt}");
                File.WriteAllBytes(fullPath, decryptedBytes);
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < 3)
                    throw new Exception("File format is invalid or incomplete.");

                string fileMethod = lines[0].Trim();
                string originalExt = lines[1].Trim();
                string encryptedText = string.Join("\n", lines.Skip(2));

                // Double check the file content method vs selected method
                if (fileMethod != method)
                {
                    //MessageBox.Show("Error: File contents indicate a different encryption method than selected.", "Method Mismatch", //MessageBoxButtons.OK, //MessageBoxIcon.Warning);
                    return;
                }

                string output = method switch
                {
                    "Caesar" => Encryption.CaesarDecrypt(encryptedText, int.Parse(userEnteredKey)),
                    "XOR" => Encryption.XorDecrypt(encryptedText, (char)(int.Parse(userEnteredKey) % 256)),
                    _ => throw new Exception("Unknown method.")
                };

                fullPath = Path.Combine(fullSaveDir, $"{fileNameBase}{originalExt}");
                File.WriteAllText(fullPath, output);
            }

            //MessageBox.Show("Decryption successful! File saved.");
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"Decryption error: {ex.Message}");
        }
    }

}