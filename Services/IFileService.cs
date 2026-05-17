using System.Threading;
using System.Threading.Tasks;

namespace l4yg0n_textytext.Services;

public interface IFileService
{
    Task<bool> ConfirmAsync(string message);
    void Exit();
    Task ShowAboutDialogAsync();
    Task<FileOpenResult?> OpenFileAsync(CancellationToken ct = default);
}

public record FileOpenResult(string Path, string Content);
