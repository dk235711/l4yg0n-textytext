using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using l4yg0n_textytext.Models.DocumentStates;
using l4yg0n_textytext.Services;
using l4yg0n_textytext.Services.LineEndings;

namespace l4yg0n_textytext.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILineEndingStrategy _lfStrategy = new LfLineEndingStrategy();
    private readonly ILineEndingStrategy _crlfStrategy = new CrlfLineEndingStrategy();
    
    public IReadOnlyList<ILineEndingStrategy> AvailableLineEndings { get; }

    [ObservableProperty] private ILineEndingStrategy _selectedLineEndingStrategy;

    [ObservableProperty] private string _lineEndingType;
    
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

    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService;
        _currentState = new CleanState();

        AvailableLineEndings = new List<ILineEndingStrategy> { _crlfStrategy, _lfStrategy };
        
        SelectedLineEndingStrategy = OperatingSystem.IsWindows() ? _crlfStrategy : _lfStrategy;
        LineEndingType = SelectedLineEndingStrategy.DisplayName;
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
        UpdateCursorPos();
    }

    partial void OnCaretIndexChanged(int value)
    {
        UpdateCursorPos();
    }

    // Valtas kulonbozo sorvegek kozott
    partial void OnSelectedLineEndingStrategyChanged(ILineEndingStrategy value)
    {
        LineEndingType = value.DisplayName;
        FileText = value.NormalizeLineEndings(FileText);
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
        SelectedLineEndingStrategy =
            LineEndingDetector.Detect(result.Content, _lfStrategy, _crlfStrategy, SelectedLineEndingStrategy);
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
            var normalizedText = SelectedLineEndingStrategy.NormalizeLineEndings(FileText);
            await _fileService.SaveFileAsync(_currentFilePath, normalizedText);
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
        var normalizedText = SelectedLineEndingStrategy.NormalizeLineEndings(FileText);
        var result = await _fileService.SaveFileAsAsync(normalizedText);
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