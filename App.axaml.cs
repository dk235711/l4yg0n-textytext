using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using l4yg0n_textytext.ViewModels;
using l4yg0n_textytext.Views;
using l4yg0n_textytext.Services;

namespace l4yg0n_textytext;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new FileService()),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
