using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;

namespace _4080capstone;

public partial class TextInputWindow : Window
{
    private static TextInputWindow? _instance;
    public bool Result { get; private set; }

    public static TextInputWindow GetInstance(string savedInput = "")
    {
        if (_instance == null)
        {
            _instance = new TextInputWindow();
            _instance.UserInput.Text = savedInput;
        }
        return _instance;
    }

    public TextInputWindow() // this is public so that the Avalonia designer doesn't complain
    {
        if (_instance == null)
            InitializeComponent();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Hide();
        //MessageBox.Show("Saved text.");
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Result = false;
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
        Result = false;
        Hide();
        e.Cancel = true; // Prevent closing
    }
}