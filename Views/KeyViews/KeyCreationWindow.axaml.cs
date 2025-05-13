using _4080capstone.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace _4080capstone;

public partial class KeyCreationWindow : Window
{
    IMsBox<ButtonResult> box;
    private TaskCompletionSource<bool?>? _tcs;
    public bool Result { get; private set; }

    public KeyCreationWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    public Task<bool?> ShowDialogAsync(Window parent, string name = "")
    {
        Identity.Text = name;
        _tcs = new TaskCompletionSource<bool?>();
        ShowDialog(parent); // modal
        return _tcs.Task;
    }

    private async void OKButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Passphrase.Text) || string.IsNullOrWhiteSpace(Identity.Text))
        {
            box = MessageBoxManager
                  .GetMessageBoxStandard("Error", $"Please enter an identifier and password.",
                            ButtonEnum.Ok);
            await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
            _tcs?.TrySetResult(false);
            return;
        } else {
            try
            {
                KeyGenerator.GenerateKeyPair(Identity.Text, Passphrase.Text);
                _tcs?.TrySetResult(true);
                Close();
            }
            catch (Exception ex)
            {
                box = MessageBoxManager
                      .GetMessageBoxStandard("Error", $"Encryption error: {ex.Message}",
                                ButtonEnum.Ok);
                await box.ShowWindowDialogAsync(TopLevel.GetTopLevel(this) as Window);
                _tcs?.TrySetResult(false);
                Close();
            }
        }

            
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!_tcs?.Task.IsCompleted ?? false)
        {
            _tcs?.TrySetResult(false); // Treat window close as cancel
        }
        base.OnClosing(e);
    }
}