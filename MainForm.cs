using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EncryptorApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            cmbMethod.Items.AddRange(new string[] { "Caesar", "XOR", "AES" });
            cmbMethod.SelectedIndex = 0;
            cmbDecrypt.Items.AddRange(new string[] { "Caesar", "XOR", "AES" });
            cmbDecrypt.SelectedIndex = 0;

            // Ensure read-only for file path fields
            txtFilePath.ReadOnly = true;
            txtDecryptFile.ReadOnly = true;
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
            string input;

            if (rbText.Checked)
            {
                input = txtInput.Text;
            }
            else if (rbFile.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
                {
                    MessageBox.Show("Please select a valid file to encrypt.");
                    return;
                }
                input = File.ReadAllText(txtFilePath.Text);
            }
            else
            {
                MessageBox.Show("Please select an input type.");
                return;
            }

            string method = cmbMethod.SelectedItem.ToString();
            string output = "";

            try
            {
                switch (method)
                {
                    case "Caesar":
                        output = Encryption.CaesarEncrypt(input);
                        break;
                    case "XOR":
                        output = Encryption.XorEncrypt(input);
                        break;
                    case "AES":
                        if (string.IsNullOrWhiteSpace(txtKey.Text))
                            throw new Exception("Key is required for AES encryption.");
                        output = Encryption.AESEncrypt(input, txtKey.Text);
                        break;
                }

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
                    throw new Exception("Incorrect decryption method selected!");

                string output = selected switch
                {
                    "Caesar" => Encryption.CaesarDecrypt(encryptedText),
                    "XOR" => Encryption.XorDecrypt(encryptedText),
                    "AES" => Encryption.AESDecrypt(encryptedText, txtKey.Text),
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
