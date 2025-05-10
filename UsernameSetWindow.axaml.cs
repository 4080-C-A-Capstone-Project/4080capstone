using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Windows.Input;

namespace _4080capstone;

public partial class UsernameSetWindow : Window
{
    private static UsernameSetWindow? _instance;

    public static UsernameSetWindow GetInstance(string savedInput = "")
    {
        if (_instance == null)
        {
            _instance = new UsernameSetWindow();
            _instance.UsernameInput.Text = savedInput;
        }
        return _instance;
    }

    public UsernameSetWindow() // this is public so that the Avalonia designer doesn't complain
    {
        if (_instance == null)
            InitializeComponent();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        Hide();
    }

    public new void Show()
    {
        if (!IsVisible)
            base.Show();
        else
            Activate();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Hide();
        e.Cancel = true; // Prevent closing
    }
}