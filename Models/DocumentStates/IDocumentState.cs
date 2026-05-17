using System.Threading.Tasks;
using l4yg0n_textytext.Services;
using l4yg0n_textytext.ViewModels;

namespace l4yg0n_textytext.Models.DocumentStates;

public interface IDocumentState
{
    bool IsDirty { get; }

    // A fajlnev utani jel ' *' vagy ures
    string GetTitlePostfix();

    void HandleTextChanged(MainWindowViewModel context);
    void HandleSaved(MainWindowViewModel context);

    // Kell-e ConfirmDiscardAsync elveteskor?
    Task<bool> ConfirmDiscardAsync(IFileService fileService);
}