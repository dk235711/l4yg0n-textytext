using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using l4yg0n_textytext.Models.DocumentStates;
using l4yg0n_textytext.Services;

namespace l4yg0n_textytext.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileService _fileService;
    [ObservableProperty] private int _caretIndex;

    private string? _currentFilePath;

    private IDocumentState _currentState;

    [ObservableProperty] private string _cursorPos = "Ln 1, Col 1";
    [ObservableProperty] private string _encoding = "Unicode (UTF-8)";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WindowTitle))]
    private string? _fileDisplayName;

    // ObservableProperty konvencio: private _varName --> hivatkozva VarName
    [ObservableProperty] private string? _fileText;

    [ObservableProperty] private string _lineEndingType = OperatingSystem.IsWindows() ? "Windows (CRLF)" : "UNIX (LF)";

    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService;
        _currentState = new CleanState();
    }

    public bool IsDirty => _currentState.IsDirty;

    public string WindowTitle => FileDisplayName is not null
        ? $"{FileDisplayName}{_currentState.GetTitlePostfix()} - TextyText"
        : $"Untitled{_currentState.GetTitlePostfix()} - TextyText";

    public void TransitionToState(IDocumentState state)
    {
        _currentState = state;

        // UI ertesitese
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(WindowTitle));
    }

    // Avalonia builtin-t extendelunk
    partial void OnFileTextChanged(string? value)
    {
        _currentState.HandleTextChanged(this);
        DetectLineEnding(value);
        UpdateCursorPos();
    }

    partial void OnCaretIndexChanged(int value)
    {
        UpdateCursorPos();
    }

    private void DetectLineEnding(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            LineEndingType = OperatingSystem.IsWindows() ? "Windows (CRLF)" : "UNIX (LF)";
            return;
        }

        LineEndingType = text.Contains("\r\n") ? "Windows (CRLF)" : "UNIX (LF)";
    }

    private void UpdateCursorPos()
    {
        var text = FileText ?? string.Empty;
        var caret = Math.Min(CaretIndex, text.Length);

        if (caret <= 0)
        {
            CursorPos = "Ln 1, Col 1";
            return;
        }

        var textBefore = text[..caret];
        var line = textBefore.Count(c => c == '\n') + 1;
        var lastNewline = textBefore.LastIndexOf('\n');
        var col = lastNewline >= 0 ? caret - lastNewline : caret + 1;

        CursorPos = $"Ln {line}, Col {col}";
    }

    [RelayCommand]
    private async Task NewFile()
    {
        // Kivettuk a ViewModelbol a logikat, az aktualis State biztositja
        if (!await _currentState.ConfirmDiscardAsync(_fileService)) return;

        FileText = null;
        _currentFilePath = null;
        FileDisplayName = null;
        CaretIndex = 0;
        Encoding = "Unicode (UTF-8)";

        TransitionToState(new CleanState());
    }

    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        if (!await _currentState.ConfirmDiscardAsync(_fileService)) return;

        var result = await _fileService.OpenFileAsync(token);
        if (result is null) return;

        FileText = result.Content;
        _currentFilePath = result.Path;
        FileDisplayName = Path.GetFileName(result.Path);
        Encoding = result.Encoding;

        TransitionToState(new CleanState());
    }

    [RelayCommand]
    private async Task SaveFile()
    {
        if (_currentFilePath is not null)
        {
            await _fileService.SaveFileAsync(_currentFilePath, FileText ?? string.Empty);
            _currentState.HandleSaved(this);
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
        _currentState.HandleSaved(this);
    }

    [RelayCommand]
    private async Task About()
    {
        await _fileService.ShowAboutDialogAsync();
    }

    [RelayCommand]
    private async Task Exit()
    {
        // Ha false ter vissza, akkor nem lepunk ki megsem
        if (!await _currentState.ConfirmDiscardAsync(_fileService)) return;
        _fileService.Exit();
    }
}