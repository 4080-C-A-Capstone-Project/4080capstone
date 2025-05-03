using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace EncryptorApp
{
    public partial class MainForm : Form
    {
        private string currentUsername = "";
        private Dictionary<string, string> userKeys = new();

        public MainForm()
        {
            InitializeComponent();
            cmbMethod.Items.AddRange(new string[] { "Caesar", "XOR", "AES" });
            cmbMethod.SelectedIndex = 0;
            cmbDecrypt.Items.AddRange(new string[] { "Caesar", "XOR", "AES" });
            cmbDecrypt.SelectedIndex = 0;

            txtFilePath.ReadOnly = true;
            txtDecryptFile.ReadOnly = true;
            txtKey.ReadOnly = true;

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

            MessageBox.Show($"Loaded last user: {currentUsername}", "Welcome");

        }

        private void SetUsername_Click(object sender, EventArgs e)
        {
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
            MessageBox.Show($"Keys saved to keys.aes for user: {currentUsername}");

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

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file to encrypt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private void btnBrowseDecryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file to decrypt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtDecryptFile.Text = openFileDialog.FileName;
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string input = "";
            string method = cmbMethod.SelectedItem.ToString();
            bool isFileInput = rbFile.Checked;
            string keyToUse = "";
            string output = "";

            try
            {
                if (rbText.Checked)
                {
                    input = txtInput.Text;
                }
                else if (isFileInput)
                {
                    string filePath = txtFilePath.Text;

                    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    {
                        MessageBox.Show("Please select a valid file to encrypt.");
                        return;
                    }

                    // Caesar and XOR only allowed for .txt files
                    if ((method == "Caesar" || method == "XOR") && Path.GetExtension(filePath).ToLower() != ".txt")
                    {
                        MessageBox.Show($"{method} encryption only supports .txt files. Use AES for other file types.");
                        return;
                    }

                    // Read file as text (works for .txt and AES-safe binary/text)
                    input = File.ReadAllText(filePath);
                }
                else
                {
                    MessageBox.Show("Please select an input type.");
                    return;
                }

                switch (method)
                {
                    case "Caesar":
                        keyToUse = GenerateFinalCaesarKey();
                        txtKey.Text = keyToUse;
                        output = Encryption.CaesarEncrypt(input, int.Parse(keyToUse));
                        break;
                    case "XOR":
                        keyToUse = GenerateFinalXorKey();
                        txtKey.Text = keyToUse;
                        output = Encryption.XorEncrypt(input, (char)(int.Parse(keyToUse) % 256));
                        break;
                    case "AES":
                        keyToUse = GenerateFinalAesKey();
                        txtKey.Text = keyToUse;
                        output = Encryption.AESEncrypt(input, keyToUse);
                        break;
                }


                // Ask user where to save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Encrypted File";
                saveFileDialog.FileName = "encrypted.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, $"{method}\n{output}");
                    MessageBox.Show("Encrypted file saved successfully.");
                }
                else
                {
                    MessageBox.Show("Encryption completed, but no file was saved.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Encryption error: {ex.Message}");
            }
        }


        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(txtDecryptFile.Text))
                {
                    MessageBox.Show("Decryption file not found.");
                    return;
                }

                string[] lines = File.ReadAllLines(txtDecryptFile.Text);
                string method = lines[0];
                string encryptedText = string.Join("\n", lines.Skip(1));
                string selected = cmbDecrypt.SelectedItem.ToString();

                if (method != selected)
                {
                    MessageBox.Show("Incorrect decryption method selected.");
                    return;
                }

                string userEnteredKey = txtDecryptKey.Text.Trim();
                if (string.IsNullOrWhiteSpace(userEnteredKey))
                {
                    MessageBox.Show("Please enter the decryption key.");
                    return;
                }

                string output = selected switch
                {
                    "Caesar" => Encryption.CaesarDecrypt(encryptedText, int.Parse(userEnteredKey)),
                    "XOR" => Encryption.XorDecrypt(encryptedText, (char)(int.Parse(userEnteredKey) % 256)),
                    "AES" => Encryption.AESDecrypt(encryptedText, userEnteredKey),
                    _ => throw new Exception("Unknown method selected.")
                };

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Decrypted File";
                saveFileDialog.FileName = "decrypted.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, output);
                    MessageBox.Show("Decryption successful! File saved.");
                }
                else
                {
                    MessageBox.Show("Decryption completed, but no file was saved.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Decryption error: {ex.Message}");
            }
        }


    }
}
