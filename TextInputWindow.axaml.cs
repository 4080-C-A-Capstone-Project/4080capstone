using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace _4080capstone;

public partial class TextInputWindow : Window
{
    public bool Result { get; private set; }

    public TextInputWindow(string savedInput="")
    {
        InitializeComponent();
        UserInput.Text = savedInput;
    }
    
    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}