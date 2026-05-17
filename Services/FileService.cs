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

// A legtobb itteni method az Avalonia QuickGuide-jaira epul:
// https://github.com/AvaloniaUI/AvaloniaUI.QuickGuides/tree/main/FileOps
public sealed class FileService : IFileService
{
    // Interface metodusai
    public async Task<FileOpenResult?> OpenFileAsync(CancellationToken ct = default)
    {
        var file = await PickOpenFileAsync();
        if (file is null) return null;

        var props = await file.GetBasicPropertiesAsync();

        // Ide majd teszunk egy dialog boxot tul nagy fajlok eseten

        var stream = await file.OpenReadAsync();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(ct);
        var encoding = reader.CurrentEncoding.EncodingName;

        return new FileOpenResult(file.Path.LocalPath, content, encoding);
    }

    public async Task SaveFileAsync(string path, string content, CancellationToken ct = default)
    {
        await File.WriteAllTextAsync(path, content, ct);
    }

    public async Task<FileSaveResult?> SaveFileAsAsync(string content, CancellationToken ct = default)
    {
        var file = await PickSaveFileAsync();
        if (file is null) return null;
        
        var path = file.Path.LocalPath;
        await SaveFileAsync(path, content, ct);

        return new FileSaveResult(path, file.Name);
    }

    public async Task<bool> ConfirmAsync(string message)
    {
        var owner = GetMainWindow();
        var dialog = new ConfirmationDialog(message);
        return await dialog.ShowDialog<bool>(owner);
    }
    
    public async Task ShowAboutDialogAsync()
    {
        var owner = GetMainWindow();
        var dialog = new AboutWindow();
        await dialog.ShowDialog(owner);
    }
    
    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();   
        }
    }
    
    // Ezeket majd kulon kellene szervezni
    private static async Task<IStorageFile?> PickOpenFileAsync()
    {
        var provider = GetStorageProvider();
        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.TextPlain],
        });
        
        return files?.Count >= 1 ? files[0] : null;
    }

    private static async Task<IStorageFile?> PickSaveFileAsync()
    {
        var provider = GetStorageProvider();
        return await provider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save As",
            DefaultExtension = "txt",
            FileTypeChoices = [FilePickerFileTypes.TextPlain],
        });
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
