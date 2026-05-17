using System.Threading.Tasks;
using l4yg0n_textytext.Services;
using l4yg0n_textytext.ViewModels;

namespace l4yg0n_textytext.Models.DocumentStates;

public class DirtyState : IDocumentState
{
    public bool IsDirty => true;

    public string GetTitlePostfix()
    {
        return " *";
    }

    public void HandleTextChanged(MainWindowViewModel context)
    {
        // Dirty-bol Dirty-be nem valtunk
    }

    public void HandleSaved(MainWindowViewModel context)
    {
        // Mentes utan ujra CleanState jon
        context.TransitionToState(new CleanState());
    }

    public async Task<bool> ConfirmDiscardAsync(IFileService fileService)
    {
        return await fileService.ConfirmAsync("You have unsaved changes. Are you sure you want to discard them?");
    }
}