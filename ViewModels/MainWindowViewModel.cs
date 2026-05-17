using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using l4yg0n_textytext.Services;

namespace l4yg0n_textytext.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileService _fileService;

    [ObservableProperty] private string? _fileText;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(WindowTitle))] private bool _isDirty;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(WindowTitle))] private string? _fileDisplayName;

    public string WindowTitle => FileDisplayName is not null
        ? $"{FileDisplayName}{(IsDirty ? " *" : "")} - TextyText"
        : "TextyText";

    private string? _currentFilePath;

    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }

    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        var result = await _fileService.OpenFileAsync(token);
        if (result is null) return;

        FileText = result.Content;
        IsDirty = false;
        _currentFilePath = result.Path;
        FileDisplayName = Path.GetFileName(result.Path);
    }

    partial void OnFileTextChanged(string? value)
    {
        IsDirty = true;
    }

    [RelayCommand]
    private async Task About()
    {
        await _fileService.ShowAboutDialogAsync();
    }

    [RelayCommand]
    private void Exit()
    {
        _fileService.Exit();
    }
}
