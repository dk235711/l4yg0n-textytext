using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using l4yg0n_textytext.Views;

namespace l4yg0n_textytext.Services;

public sealed class FileService : IFileService
{
    public async Task<FileOpenResult?> OpenFileAsync(CancellationToken ct = default)
    {
        var file = await PickOpenFileAsync();
        if (file is null) return null;

        var props = await file.GetBasicPropertiesAsync();

        // Ide majd teszunk egy dialog boxot tul nagy fajlok eseten

        await using var stream = await file.OpenReadAsync();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(ct);

        return new FileOpenResult(file.Path.LocalPath, content);
    }

    private static async Task<IStorageFile?> PickOpenFileAsync()
    {
        var provider = GetStorageProvider();
        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Textfile",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.TextPlain],
        });

        return files?.Count >= 1 ? files[0] : null;
    }

    public async Task<bool> ConfirmAsync(string message)
    {
        var owner = GetMainWindow();
        var dialog = new ConfirmationDialog(message);
        return await dialog.ShowDialog<bool>(owner);
    }
    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();   
        }
    }

    public async Task ShowAboutDialogAsync()
    {
        var owner = GetMainWindow();
        var dialog = new AboutWindow();
        await dialog.ShowDialog(owner);
    }

    private static IStorageProvider GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow?.StorageProvider is { } provider)
            return provider;

        throw new NullReferenceException("Missing StorageProvider instance.");
    }

    private static Window GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow!;

        throw new NullReferenceException("Missing MainWindow instance.");
    }
}
