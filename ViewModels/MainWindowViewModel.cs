using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using l4yg0n_textytext.Services;

namespace l4yg0n_textytext.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    private readonly IFileService _fileService;

    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService;
    }

    [RelayCommand]
    private async Task About()
    {
        await _fileService.ShowAboutDialogAsync();
    }
}
