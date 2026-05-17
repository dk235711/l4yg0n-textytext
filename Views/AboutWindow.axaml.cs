using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace l4yg0n_textytext.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
        };
    }

    private void OnOk(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}