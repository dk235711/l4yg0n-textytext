using System;

namespace l4yg0n_textytext.Services.LineEndings;

public class CrlfLineEndingStrategy : ILineEndingStrategy
{
    public string DisplayName => "Windows (CRLF)";
    public string NewLine => "\r\n";
    public string NormalizeLineEndings(string? text) => (text ?? String.Empty).ReplaceLineEndings(NewLine);
    
    public override string ToString() => DisplayName;
}