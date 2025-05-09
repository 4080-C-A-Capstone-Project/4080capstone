using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using System.Diagnostics;

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
        encryptionComboBox.ItemsSource = _encryptionOptions;
        decryptionComboBox.ItemsSource = _encryptionOptions;

        rbText.IsCheckedChanged+= RbInputType_CheckedChanged;
        rbFile.IsCheckedChanged += RbInputType_CheckedChanged;

        //txtFilePath.ReadOnly = true;
        //txtDecryptFile.ReadOnly = true;
        //txtKey.ReadOnly = true;

        //LoadLastUserFromFile();
    }

    private void RbInputType_CheckedChanged(object? sender, EventArgs e)
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
        var textForm = new TextInputWindow(savedTextInput);
        textForm.Show();
        textForm.Closed += (s, args) =>
        {
            if (textForm.Result) // User clicked Save
            {
                savedTextInput = textForm.UserInput.Text ?? "";
                MessageBox.Show("Text saved.");
            }
        };
    }


}