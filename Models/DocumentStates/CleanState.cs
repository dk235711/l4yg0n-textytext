using System.Threading.Tasks;
using l4yg0n_textytext.Services;
using l4yg0n_textytext.ViewModels;

namespace l4yg0n_textytext.Models.DocumentStates;

public class CleanState : IDocumentState
{
    public bool IsDirty => false;

    public string GetTitlePostfix()
    {
        return "";
    }

    public void HandleTextChanged(MainWindowViewModel context)
    {
        // DirtyState-re valtunk
        context.TransitionToState(new DirtyState());
    }

    public void HandleSaved(MainWindowViewModel context)
    {
        // CleanState-ben vagyunk, nincs tranzicio
    }

    public Task<bool> ConfirmDiscardAsync(IFileService fileService)
    {
        // CleanState, igy nem kell confirm
        return Task.FromResult(true);
    }
}