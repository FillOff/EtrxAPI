using System.Collections.Generic;

namespace Etrx.Domain.Models;

public class Tag : Entity
{
    public string Name { get; set; } = string.Empty;
    public int Complexity { get; set; }
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
}