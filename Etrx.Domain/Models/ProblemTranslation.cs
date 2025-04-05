namespace Etrx.Core.Models;

public class ProblemTranslation
{
    public int ContestId { get; set; }
    
    public string Index { get; set; } = string.Empty;
    
    public string LanguageCode { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
}
