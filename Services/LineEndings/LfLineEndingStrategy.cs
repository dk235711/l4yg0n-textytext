using System;

namespace l4yg0n_textytext.Services.LineEndings;

public class LfLineEndingStrategy : ILineEndingStrategy
{
    public string DisplayName => "UNIX (LF)";
    public string NewLine => "\n";
    public string NormalizeLineEndings(string? text) => (text ?? String.Empty).ReplaceLineEndings(NewLine);
    
    public override string ToString() => DisplayName;
}