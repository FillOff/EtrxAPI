namespace Etrx.Domain.Models;

public class ContestTranslation
{
    public int ContestId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
