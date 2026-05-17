using System.Threading;
using System.Threading.Tasks;

namespace l4yg0n_textytext.Services;

public interface IFileService
{
    Task<FileOpenResult?> OpenFileAsync(CancellationToken ct = default);
    Task SaveFileAsync(string path, string content, CancellationToken ct = default);
    Task<FileSaveResult?> SaveFileAsAsync(string content, CancellationToken ct = default);
    Task<bool> ConfirmAsync(string message);
    Task ShowAboutDialogAsync();
    void Exit();

}

public record FileOpenResult(string Path, string Content);
public record FileSaveResult(string Path, string Name);
