namespace l4yg0n_textytext.Services.LineEndings;

public interface ILineEndingStrategy
{
    string DisplayName { get; } // UNIX (LF) vagy Windows (CRLF)
    string NewLine { get; } // \r\n vagy \n
    string NormalizeLineEndings(string? text);
}