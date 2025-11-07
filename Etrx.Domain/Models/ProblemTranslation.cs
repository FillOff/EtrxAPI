namespace Etrx.Domain.Models;

public class ProblemTranslation : Entity
{
    public Guid ProblemId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
}
