namespace Etrx.Domain.Models;

public class Tag : Entity
{
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<Problem> Problems { get; set; } = [];
}