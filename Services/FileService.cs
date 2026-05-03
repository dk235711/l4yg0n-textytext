using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using l4yg0n_textytext.Views;

namespace l4yg0n_textytext.Services;

public sealed class FileService : IFileService
{
    public async Task ShowAboutDialogAsync()
    {
        var owner = GetMainWindow();
        var dialog = new AboutWindow();
        await dialog.ShowDialog(owner);
    }

    private static Window GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow!;

        throw new NullReferenceException("Missing MainWindow instance.");
    }
}
