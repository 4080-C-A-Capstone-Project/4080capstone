using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace _4080capstone;

public partial class TextInputWindow : Window
{
    IMsBox<ButtonResult> box;
    private TaskCompletionSource<string?>? _tcs;
    public bool Result { get; private set; }

    public TextInputWindow()
    {
        InitializeComponent();
    }

    public Task<string?> ShowDialogAsync(Window parent, string savedInput = "")
    {
        UserInput.Text = savedInput;
        _tcs = new TaskCompletionSource<string?>();
        ShowDialog(parent); // modal
        return _tcs.Task;
    }


    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        string input = UserInput.Text.Trim();
        if (!string.IsNullOrWhiteSpace(input))
            _tcs?.TrySetResult(input);
        else
            _tcs?.TrySetResult(null);
        Close();

        box = MessageBoxManager
                  .GetMessageBoxStandard("Success", "Saved text.",
                            ButtonEnum.Ok);
        await box.ShowAsync();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!_tcs?.Task.IsCompleted ?? false)
        {
            _tcs?.TrySetResult(null); // Treat window close as cancel
        }
        base.OnClosing(e);
    }
}