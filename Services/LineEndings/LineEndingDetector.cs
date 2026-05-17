namespace l4yg0n_textytext.Services.LineEndings;

public class LineEndingDetector
{
    public static ILineEndingStrategy Detect(
        string? text,
        ILineEndingStrategy lfStrategy,
        ILineEndingStrategy crlfStrategy,
        ILineEndingStrategy fallbackStrategy)
    {
        if (string.IsNullOrEmpty(text)) return fallbackStrategy;
        
        return text.Contains("\r\n") ? crlfStrategy : lfStrategy;
    }
}