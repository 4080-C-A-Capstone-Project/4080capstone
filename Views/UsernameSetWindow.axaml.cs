using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Windows.Input;

namespace _4080capstone.Views;

public partial class UsernameSetWindow : Window
{
    private TaskCompletionSource<string?>? _tcs;

    public UsernameSetWindow()
    {
        InitializeComponent();
    }

    public Task<string?> ShowDialogAsync(Window parent, string savedInput = "")
    {
        UsernameInput.Text = savedInput;
        _tcs = new TaskCompletionSource<string?>();
        ShowDialog(parent); // modal
        return _tcs.Task;
    }

    private void SetUsernameBtn_Click(object? sender, RoutedEventArgs e)
    {
        string input = UsernameInput.Text.Trim();
        if (!string.IsNullOrWhiteSpace(input))
            _tcs?.TrySetResult(input);
        else
            _tcs?.TrySetResult(null);
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
