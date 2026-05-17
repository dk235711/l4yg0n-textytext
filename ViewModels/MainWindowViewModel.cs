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

    // ObservableProperty convencio: private _varName --> hivatkozva VarName
    [ObservableProperty] private string? _fileText;
    // Modosult-e a fajl tartalma, miota megnyitottuk?
    [ObservableProperty][NotifyPropertyChangedFor(nameof(WindowTitle))] private bool _isDirty;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(WindowTitle))] private string? _fileDisplayName;
    
    public string WindowTitle => FileDisplayName is not null
        ? $"{FileDisplayName}{(IsDirty ? " *" : "")} - TextyText"
        : $"Untitled{(IsDirty ? " *" : "")} - TextyText";

    private string? _currentFilePath;

    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    // Avalonia builtin-t extendelunk
    partial void OnFileTextChanged(string? value)
    {
        IsDirty = true;
    }

    [RelayCommand]
    private async Task NewFile()
    {
        if (!await ConfirmDiscardAsync()) return;

        FileText = null;
        _currentFilePath = null;
        FileDisplayName = null;
        IsDirty = false;
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

    [RelayCommand]
    private async Task SaveFile()
    {
        if (_currentFilePath is not null)
        {
            await _fileService.SaveFileAsync(_currentFilePath, FileText ?? string.Empty);
            IsDirty = false;
        }
        else
        {
            await SaveFileAs();
        }
    }

    [RelayCommand]
    private async Task SaveFileAs()
    {
        var result = await _fileService.SaveFileAsAsync(FileText ?? string.Empty);
        if (result is null) return;
        
        _currentFilePath = result.Path;
        FileDisplayName = result.Name;
        IsDirty = false;
    }
    
    [RelayCommand]
    private async Task About()
    {
        await _fileService.ShowAboutDialogAsync();
    }

    // Ha barmi valtoztatas tortent, akkor felhasznaloi konfirmaciot varunk a jovahagyashoz.
    private async Task<bool> ConfirmDiscardAsync()
    {
        return !IsDirty || await _fileService.ConfirmAsync(
            "You have unsaved changes. Are you sure you want to discard them?");
    }

    [RelayCommand]
    private async Task Exit()
    {
        // Ha false ter vissza, akkor nem lepunk ki megsem
        if (!await ConfirmDiscardAsync()) return;
        _fileService.Exit();
    }
}
