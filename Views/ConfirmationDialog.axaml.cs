using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace l4yg0n_textytext.Views;

public partial class ConfirmationDialog : Window
{
    public ConfirmationDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) Close(false);
        };
    }
    
    private void OnYes(object? sender, RoutedEventArgs e) => Close(true);
    private void OnNo(object? sender, RoutedEventArgs e) => Close(false);
}